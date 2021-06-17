using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Exceptions;
using Database.Model;
using Database.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class FileBusinessImplementation : IFileBusiness
    {
        private readonly User _requester;
        private readonly IHttpContextAccessor _context;
        private readonly IFileRepository _fileRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<AppointmentStepFile> _appointmentFileRepository;
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly FileConverter _converter;

        public FileBusinessImplementation(
            User requester, 
            IHttpContextAccessor context, 
            IFileRepository fileRepository,
            IRepository<User> userRepository,
            IRepository<AppointmentStepFile> appointmentFileRepository,
            IRepository<Appointment> appointmentRepository)
        {
            _requester = requester;
            _context = context;
            _fileRepository = fileRepository;
            _userRepository = userRepository;
            _appointmentFileRepository = appointmentFileRepository;
            _appointmentRepository = appointmentRepository;
            var url = _context.HttpContext.Request.IsHttps ? "https://" : "http://";
            url += _context.HttpContext.Request.Host;
            url += "/api/v1.0/file";
            _converter = new FileConverter(url);
        }

        public Task<FileDetail> GetFileContentByGuidAsync(Guid fileGuid)
        {
            return _fileRepository.GetFileWithContentByGuidAsync(fileGuid);
        }

        public async Task<FileDetailVO> SaveProfilePicAsync(IFormFile formFile)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                throw new APIException("Empty file provided");
            }

            var file = GetFileFromForm(formFile);

            if (file.Type.ToLower() != ".jpg" && file.Type.ToLower() != ".png" && file.Type.ToLower() != ".jpeg")
            {
                throw new APIException($"Unsupported picture file type {file.Type}");
            }
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                file.Content.Content = ms.ToArray();
            }
            file = await _fileRepository.CreateAsync(file);


            // update users profile pic
            var user = await _userRepository.FindByIdAsync(_requester.Id);
            var previousPic = user.ProfilePictureId;
            user.ProfilePictureId = file.Id;
            await _userRepository.UpdateAsync(user);
            
            // remove old profile pic
            if (previousPic.HasValue)
            {
                var fileDetail = await _fileRepository.FindByIdAsync(previousPic.Value);
                if (fileDetail != null)
                    await _fileRepository.DeleteFileByGuidAsync(new Guid(fileDetail.Guid));
            }

            return _converter.Parse(file);
        }           

        public async Task<FileDetailVO> SaveStepFileAsync(IFormFile formFile, long appointmentId, long stepId)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                throw new APIException("Empty file provided");
            }

            var file = GetFileFromForm(formFile);

            if (file.Type.ToLower() != ".pdf" && file.Type.ToLower() != ".doc" && file.Type.ToLower() != ".docx")
            {
                throw new APIException($"Unsupported document file type {file.Type}");
            }

            var appointment = await _appointmentRepository.FindByIdAsync(appointmentId, 
                $"{nameof(Appointment.AppointmentSteps)}.{nameof(AppointmentStep.Step)}");

            if (appointment == null)
            {
                throw new NotFoundException($"Appointment id {appointmentId} not found");
            }

            var step = appointment.AppointmentSteps
                .Where(x => x.Step.Id == stepId)
                .FirstOrDefault();
            
            if (step == null)
            {
                throw new NotFoundException($"Step id {stepId} not found");
            }
            if (!step.Step.AllowFileUpload)
            {
                throw new APIException("The step does not allow file upload");
            }

            if (step.Step.TargetUser != _requester.Type)
            {
                throw new APIException($"Only the {step.Step.TargetUser} can upload files to this step");
            }

            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                file.Content.Content = ms.ToArray();
            }
            file = await _fileRepository.CreateAsync(file);

            _ = await _appointmentFileRepository.CreateAsync(new AppointmentStepFile { AppointmentStepId = step.Id, FileId = file.Id });

            return _converter.Parse(file);
        }

        public async Task<List<FileDetailVO>> SaveStepFilesAsync(IList<IFormFile> files, long appointmentId, long stepId)
        {
            List<FileDetailVO> list = new List<FileDetailVO>();

            foreach (var file in files)
            {
                list.Add(await SaveStepFileAsync(file, appointmentId, stepId));
            }

            return list;
        }

        public async Task DeleteFileAsync(Guid fileGuid)
        {
            var fileDetails = await _fileRepository.GetFileDetailsByGuidAsync(fileGuid);
            if (fileDetails == null)
                throw new NotFoundException("File not found");

            if (fileDetails.UploaderId != _requester.Id)
                throw new UnauthorizedException("User not allowed to delete this file");

            await _fileRepository.DeleteFileByGuidAsync(fileGuid);
        }

        private FileDetail GetFileFromForm(IFormFile form)
        {
            var fileType = Path.GetExtension(form.FileName);
            var docName = Path.GetFileName(form.FileName);
            var guid = Guid.NewGuid().ToByteArray();
            FileDetail file = new FileDetail
            {
                Guid = guid,
                Name = docName,
                Type = fileType,
                Size = form.Length,
                UploaderId = _requester.Id,
                Content = new FileContent { FileGuid = guid }
            };
            return file;
        }
    }
}
