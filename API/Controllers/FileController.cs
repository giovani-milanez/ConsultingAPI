using API.Business;
using API.Data.VO;
using API.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]    
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FileController : Controller
    {
        private readonly IFileBusiness _fileBusiness;

        public FileController(IFileBusiness fileBusiness)
        {
            _fileBusiness = fileBusiness;
        }

        [HttpPost("profilePic")]
        [RequestFormLimits(MultipartBodyLengthLimit = 16777215)]
        [Authorize]
        [ProducesResponseType((200), Type = typeof(FileDetailVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Produces("application/json")]
        public async Task<IActionResult> UploadProfilePic([FromForm] IFormFile file)
        {
            try
            {
                FileDetailVO detail = await _fileBusiness.SaveProfilePicAsync(file);
                return new OkObjectResult(detail);
            }
            catch(Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }       

        [HttpPost("appointment/{appointmentId}/step/{stepId}/document")]
        [Authorize]
        [RequestFormLimits(MultipartBodyLengthLimit = 16777215)]
        [ProducesResponseType((200), Type = typeof(FileDetailVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Produces("application/json")]
        public async Task<IActionResult> UploadStepDocument([FromForm] IFormFile file, long appointmentId, long stepId)
        {
            try
            {
                FileDetailVO detail = await _fileBusiness.SaveStepFileAsync(file, appointmentId, stepId);
                return new OkObjectResult(detail);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }
        [HttpPost("appointment/{appointmentId}/step/{stepId}/documents")]
        [Authorize]
        [RequestFormLimits(MultipartBodyLengthLimit = 16777215)]
        [ProducesResponseType((200), Type = typeof(List<FileDetailVO>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Produces("application/json")]
        public async Task<IActionResult> UploadMultipleStepDocument([FromForm] IList<IFormFile> files, long appointmentId, long stepId)
        {
            try
            {
                var details = await _fileBusiness.SaveStepFilesAsync(files, appointmentId, stepId);
                return new OkObjectResult(details);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpDelete("{fileGuid}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteFileAsync(Guid fileGuid)
        {
            try
            {
                await _fileBusiness.DeleteFileAsync(fileGuid);
                return NoContent();
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpGet("{fileGuid}")]
        [AllowAnonymous]
        [ProducesResponseType((200), Type = typeof(byte[]))]
        [ProducesResponseType(404)]
        [Produces("application/octet-stream")]
        public async Task<IActionResult> DownloadFileAsync(Guid fileGuid)
        {
            try
            {                
                var file = await _fileBusiness.GetFileContentByGuidAsync(fileGuid);
                if (file == null)
                {
                    var result = new ContentResult();
                    result.StatusCode = 404;
                    return result;
                }
                var contentType = $"application/{Path.GetExtension(file.Type).Replace(".", "")}";
                return File(file.Content.Content, contentType, file.Name);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }         
    }
}
