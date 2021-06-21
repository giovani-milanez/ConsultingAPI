using API.Business;
using API.Data.VO;
using API.Extension;
using API.Hypermedia.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly ILogger<ServiceController> _logger;
        private IServiceBusiness _business;
        public ServiceController(ILogger<ServiceController> logger, IServiceBusiness business)
        {
            _logger = logger;
            _business = business;
        }

        [HttpGet()]
        [ProducesResponseType((200), Type = typeof(List<ServiceVO>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _business.FindAllAsync());
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpGet("{sortDirection}/{pageSize}/{page}")]
        [ProducesResponseType((200), Type = typeof(List<ServiceVO>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Get(
           [FromQuery] string title,
           string sortDirection,
           int pageSize,
           int page,
           CancellationToken cancellationToken
           )
        {
            try
            {
                return Ok(await _business.FindWithPagedSearchAsync(title, sortDirection, pageSize, page, cancellationToken));
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType((200), Type = typeof(ServiceVO))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> GetAsync(long id)
        {
            try
            {
                var item = await _business.FindByIdAsync(id);
                if (item == null)
                    return this.ApiNotFoundRequest();

                return Ok(item);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType((200), Type = typeof(ServiceVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Post([FromBody] ServiceCreateVO item)
        {
            try
            {
                if (item == null) return BadRequest();
                var result = await _business.CreateAsync(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpPut]
        [ProducesResponseType((200), Type = typeof(StepVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> PutAsync([FromBody] ServiceEditVO item)
        {
            if (item == null) return this.ApiBadRequest("input is null");
            try
            {
                return Ok(await _business.UpdateAsync(item));
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                await _business.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }
    }
}
