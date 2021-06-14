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

        [HttpPost("appointment/{appointmentId}/step/{stepId}/uploadFile")]
        [Authorize]
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
        [HttpPost("appointment/{appointmentId}/step/{stepId}/uploadFiles")]
        [Authorize]
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
        public async Task<IActionResult> DeleteFileAsync(Guid guid)
        {
            try
            {
                await _fileBusiness.DeleteFileAsync(guid);
                return NoContent();
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpGet("{fileGuid}")]
        [ProducesResponseType((200), Type = typeof(byte[]))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Produces("application/octet-stream")]
        public async Task<IActionResult> DownloadFileAsync(Guid fileGuid)
        {
            try
            {                
                var file = await _fileBusiness.GetFileByGuidAsync(fileGuid);
                if (file == null)
                {
                    return this.ApiNotFoundRequest($"File not found");
                }
                HttpContext.Response.ContentType = 
                    $"application/{Path.GetExtension(file.Type).Replace(".", "")}";
                HttpContext.Response.Headers.Add("content-length", file.Size.ToString());

                await HttpContext.Response.Body.WriteAsync(file.Content, 0, (int)file.Size);
                return new ContentResult();
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }         
    }
}
