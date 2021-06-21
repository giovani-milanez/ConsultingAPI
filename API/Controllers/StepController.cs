using API.Business;
using API.Data.VO;
using API.Extension;
using API.Hypermedia.Filters;
using Database.Utils;
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
    public class StepController : ControllerBase
    {
        private readonly ILogger<StepController> _logger;
        private IStepBusiness _business;
        public StepController(ILogger<StepController> logger, IStepBusiness business)
        {
            _logger = logger;
            _business = business;
        }

        [HttpGet()]
        [ProducesResponseType((200), Type = typeof(List<StepVO>))]
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

        [HttpGet("{page}/{pageSize}")]
        [ProducesResponseType((200), Type = typeof(List<StepVO>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Get(
           int page,
           int pageSize,
           [FromQuery] string nameFilter,
           [FromQuery] string sortField,
           CancellationToken cancellationToken
           )
        {
            try
            {
                PagedRequest pagedRequest = new PagedRequest(page, pageSize);

                if (!String.IsNullOrEmpty(nameFilter))
                {
                    pagedRequest.Filters.Add(new Filter { FieldName = "display_name", Value = nameFilter });
                }
                if (!String.IsNullOrEmpty(sortField))
                {
                    string order = sortField.EndsWith("_desc") ? "desc" : "asc";
                    pagedRequest.SortFields.Add(new SortField { FieldName = sortField, SortOrder = order });
                }

                return Ok(await _business.FindWithPagedSearchAsync(pagedRequest, cancellationToken));
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType((200), Type = typeof(StepVO))]
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
        [ProducesResponseType((200), Type = typeof(StepVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Post([FromBody] StepCreateVO item)
        {
            if (item == null) return this.ApiBadRequest("input is null");
            try
            {
                return Ok(await _business.CreateAsync(item));
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
        public async Task<IActionResult> Put([FromBody] StepEditVO item)
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
