using API.Business;
using API.Data.VO;
using API.Data.VO.User;
using API.Extension;
using API.Hypermedia.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PublicController : ControllerBase
    {
        private readonly IPublicBusiness _business;

        public PublicController(IPublicBusiness business)
        {
            _business = business;
        }

        [HttpGet("find-services/{sortDirection}/{pageSize}/{page}")]
        [ProducesResponseType((200), Type = typeof(List<ServiceVO>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> GetServices(
          [FromQuery] string name,
          string sortDirection,
          int pageSize,
          int page,
          CancellationToken cancellationToken
          )
        {
            try
            {
                return Ok(await _business.FindWithPagedSearchAsync(name, sortDirection, pageSize, page, cancellationToken));
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpGet("consultant/{id}")]
        [ProducesResponseType((200), Type = typeof(ConsultantVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> GetConsultant(int id, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _business.GetConsultantByIdAsync(id));
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }
    }
}
