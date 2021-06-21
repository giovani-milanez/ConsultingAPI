using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Exceptions;
using API.Hypermedia.Utils;
using Database.Extension;
using Database.Model;
using Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class AppointmentBusinessImplementation : IAppointmentBusiness
    {
        private readonly User _requester;
        private readonly IAppointmentRepository _repository;
        private readonly IRepository<User> _userRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IFileRepository _fileRepository;
        private readonly AppointmentConverter _converter;
        private readonly AppointmentStepConverter _stepConverter;

        public AppointmentBusinessImplementation(
            User requester, 
            IAppointmentRepository repository, 
            IRepository<User> userRepository, 
            IServiceRepository serviceRepository,
            IFileRepository fileRepository,
            FileConverter fileConverter)
        {
            _requester = requester;
            _repository = repository;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _fileRepository = fileRepository;
            _converter = new AppointmentConverter(fileConverter);
            _stepConverter = new AppointmentStepConverter(fileConverter);
        }

        public async Task<AppointmentVO> CreateAsync(AppointmentCreateVO vo)
        {
            if (!_requester.IsConsultantOrAdmin())
            {
                throw new UnauthorizedException("User must be consultant to create a service");
            }

            var service = await _serviceRepository.FindByIdAsync(vo.ServiceId, false, nameof(Service.ServicesSteps));

            if (service == null)
            {
                throw new NotFoundException($"Service id {vo.ServiceId} not found");
            }

            if (!_requester.IsAdmin() && service.UserId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to create an appointment for this service");
            }

            var client = await _userRepository.FindByIdAsync(vo.ClientId, false);

            if (client == null)
            {
                throw new NotFoundException($"Client id {vo.ClientId} not found");
            }

            if (client.Id == _requester.Id)
            {
                throw new APIException("Cannot create appointment to itself");
            }

            var entity = _converter.Parse(vo);
            entity.AppointmentSteps = service.ServicesSteps.Select(x => new AppointmentStep
            {
                StepId = x.StepId,
                IsCompleted = false,
                SubmitData = "{}"
            }).ToList();

            entity = await _repository.CreateAsync(entity);
            return await this.FindByIdAsync(entity.Id);
        }

        public async Task<AppointmentStepVO> SubmitStep(AppointmentStepSubmitVO submit)
        {
            var entity = await _repository.FindByIdAsync(submit.AppointmentId, true,
                nameof(Appointment.AppointmentSteps),
                $"{nameof(Appointment.AppointmentSteps)}.{nameof(AppointmentStep.Step)}");

            if (entity == null)
            {
                throw new NotFoundException($"Appointment id {submit.AppointmentId} not found");
            }

            if (!_requester.IsAdmin() && entity.ClientId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to submit this step on this appointment");
            }

            var step = entity.AppointmentSteps.FirstOrDefault(x => x.StepId == submit.StepId);
            if (step == null)
            {
                throw new NotFoundException($"Step id {submit.StepId} of appointment id {submit.AppointmentId} not found");
            }

            if (step.IsCompleted)
            {
                throw new APIException("Step already completed");
            }

            if (!_requester.IsAdmin() && step.Step.TargetUser != _requester.Type)
            {
                throw new UnauthorizedException($"Only the {step.Step.TargetUser} user can submit this step");
            }

            // TODO: Check against json-schema
            step = await _repository.SubmitStep(entity, submit.StepId, submit.SubmitData);
            return _stepConverter.Parse(step);
        }

        public async Task<AppointmentStepVO> EditStep(AppointmentStepSubmitVO submit)
        {
            var entity = await _repository.FindByIdAsync(submit.AppointmentId, true,
                nameof(Appointment.AppointmentSteps),
                $"{nameof(Appointment.AppointmentSteps)}.{nameof(AppointmentStep.Step)}");

            if (entity == null)
            {
                throw new NotFoundException($"Appointment id {submit.AppointmentId} not found");
            }

            if (!_requester.IsAdmin() && entity.ClientId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to submit this step on this appointment");
            }

            var step = entity.AppointmentSteps.FirstOrDefault(x => x.StepId == submit.StepId);
            if (step == null)
            {
                throw new NotFoundException($"Step id {submit.StepId} of appointment id {submit.AppointmentId} not found");
            }

            if (!step.IsCompleted)
            {
                throw new APIException("Step not completed yet");
            }

            if (!_requester.IsAdmin() && step.Step.TargetUser != _requester.Type)
            {
                throw new UnauthorizedException($"Only the {step.Step.TargetUser} user can submit this step");
            }

            // TODO: Check against json-schema

            step = await _repository.EditStep(entity, submit.StepId, submit.SubmitData);
            return _stepConverter.Parse(step);
        }

        public async Task DeleteAsync(long id)
        {
            if (!_requester.IsAdmin())
            {
                throw new UnauthorizedException("User must be admin to delete an appointment");
            }
            var entity = await _repository.FindByIdAsync(id, true);

            if (entity == null)
            {
                throw new NotFoundException($"Can't find appointment of id {id}");
            }

            await _repository.DeleteTrackedAsync(entity);
        }

        public async Task<List<AppointmentVO>> FindAllAsync()
        {
            var all = await _repository.FindAllAsync(_requester,
                    $"{nameof(Appointment.Service)}.{nameof(Service.User)}.{nameof(User.ProfilePicture)}",
                    $"{nameof(Appointment.Service)}.{nameof(Service.ServicesSteps)}.{nameof(ServicesStep.Step)}",
                    $"{nameof(Appointment.AppointmentSteps)}.{nameof(AppointmentStep.AppointmentStepFiles)}.{nameof(AppointmentStepFile.File)}",
                    $"{nameof(Appointment.Client)}.{nameof(User.ProfilePicture)}",
                    nameof(Appointment.Rating)
                );
            //foreach (var item in all)
            //{
            //    await this.AddFilesAsync(item);
            //}
            return _converter.Parse(all);
        }

        public async Task<AppointmentVO> FindByIdAsync(long id)
        {
            var entity = await _repository.FindByIdAsync(id, false,
                    $"{nameof(Appointment.Service)}.{nameof(Service.User)}.{nameof(User.ProfilePicture)}",
                    $"{nameof(Appointment.Service)}.{nameof(Service.ServicesSteps)}.{nameof(ServicesStep.Step)}",
                    $"{nameof(Appointment.AppointmentSteps)}.{nameof(AppointmentStep.AppointmentStepFiles)}.{nameof(AppointmentStepFile.File)}",
                    $"{nameof(Appointment.Client)}.{nameof(User.ProfilePicture)}",
                    nameof(Appointment.Rating)
                );

            if (entity == null)
            {
                throw new NotFoundException($"Appointment id {id} not found");
            }

            if (!_requester.IsAdmin() && (entity.ClientId != _requester.Id && entity.Service.UserId != _requester.Id))
            {
                throw new UnauthorizedException("User is not allowed to view this appointment");
            }
            //await this.AddFilesAsync(entity);
            return _converter.Parse(entity);
        }

        public async Task<PagedSearchVO<AppointmentVO>> FindWithPagedSearchAsync(string clientName, string sortDirection, int pageSize, int page, CancellationToken cancellationToken)
        {
            var result = await _repository.FindWithPagedSearchAsync(
                clientName, 
                _requester, 
                sortDirection, 
                pageSize, 
                page, 
                cancellationToken,
                $"{nameof(Appointment.Service)}.{nameof(Service.User)}.{nameof(User.ProfilePicture)}",
                $"{nameof(Appointment.Service)}.{nameof(Service.ServicesSteps)}.{nameof(ServicesStep.Step)}",
                $"{nameof(Appointment.AppointmentSteps)}.{nameof(AppointmentStep.AppointmentStepFiles)}.{nameof(AppointmentStepFile.File)}",
                $"{nameof(Appointment.Client)}.{nameof(User.ProfilePicture)}",
                nameof(Appointment.Rating));

            return new PagedSearchVO<AppointmentVO>
            {
                CurrentPage = result.CurrentPage,
                List = _converter.Parse(result.Items),
                PageSize = result.PageSize,
                TotalResults = result.TotalResults
            };
        }
    }
}
