using API.Business;
using API.Data.VO;
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
        public async Task<IActionResult> Get(
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
    }
}
