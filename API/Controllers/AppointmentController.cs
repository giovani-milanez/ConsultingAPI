using API.Business;
using API.Data.VO;
using API.Extension;
using API.Hypermedia.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("Bearer")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly ILogger<AppointmentController> _logger;
        private IAppointmentBusiness _business;
        public AppointmentController(ILogger<AppointmentController> logger, IAppointmentBusiness business)
        {
            _logger = logger;
            _business = business;
        }

        [HttpGet()]
        [ProducesResponseType((200), Type = typeof(List<AppointmentVO>))]
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
        [ProducesResponseType((200), Type = typeof(List<AppointmentVO>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Get(
           [FromQuery] string clientName,
           string sortDirection,
           int pageSize,
           int page,
           CancellationToken cancellationToken
           )
        {
            try
            {
                return Ok(await _business.FindWithPagedSearchAsync(clientName, sortDirection, pageSize, page, cancellationToken));
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType((200), Type = typeof(AppointmentVO))]
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
        [ProducesResponseType((200), Type = typeof(AppointmentVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Post([FromBody] AppointmentCreateVO item)
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

        [HttpPost("{id}/step/{stepId}/submit")]
        [ProducesResponseType((200), Type = typeof(AppointmentStepVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> StepSubmit(long id, long stepId, [FromBody] JsonDocument submitData)
        {
            try
            {
                var item = new AppointmentStepSubmitVO {AppointmentId = id, StepId = stepId, SubmitData = submitData };
                var result = await _business.SubmitStep(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpPut("{id}/step/{stepId}/edit")]
        [ProducesResponseType((200), Type = typeof(AppointmentStepVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> StepEdit(long id, long stepId, [FromBody] JsonDocument submitData)
        {
            try
            {
                var item = new AppointmentStepSubmitVO { AppointmentId = id, StepId = stepId, SubmitData = submitData };
                var result = await _business.EditStep(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        //[HttpPut]
        //[ProducesResponseType((200), Type = typeof(StepVO))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[TypeFilter(typeof(HyperMediaFilter))]
        //public async Task<IActionResult> PutAsync([FromBody] ServiceEditVO item)
        //{
        //    if (item == null) return this.ApiBadRequest("input is null");
        //    try
        //    {
        //        return Ok(await _business.UpdateAsync(item));
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.ApiResulFromException(ex);
        //    }
        //}

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
