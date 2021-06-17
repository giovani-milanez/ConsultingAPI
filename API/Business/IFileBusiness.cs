using API.Data.VO;
using Database.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Business
{
    public interface IFileBusiness
    {
        Task<FileDetail> GetFileContentByGuidAsync(Guid fileGuid);
        Task<FileDetailVO> SaveProfilePicAsync(IFormFile file);
        Task<List<FileDetailVO>> SaveStepFilesAsync(IList<IFormFile> files, long appointmentId, long stepId);
        Task<FileDetailVO> SaveStepFileAsync(IFormFile file, long appointmentId, long stepId);
        Task DeleteFileAsync(Guid fileGuid);
    }
}
