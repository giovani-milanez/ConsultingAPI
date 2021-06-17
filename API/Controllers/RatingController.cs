using API.Business;
using API.Data.VO.Rating;
using API.Extension;
using API.Hypermedia.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly ILogger<RatingController> _logger;
        private IRatingBusiness _business;
        public RatingController(ILogger<RatingController> logger, IRatingBusiness business)
        {
            _logger = logger;
            _business = business;
        }

        [HttpGet("{id}")]
        [ProducesResponseType((200), Type = typeof(RatingVO))]
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

        [HttpGet("byConsultant/{consultantId}")]
        [ProducesResponseType((200), Type = typeof(List<RatingVO>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> GetAllByConsultantAsync(long consultantId)
        {
            try
            {
                var items = await _business.FindAllByConsultantIdAsync(consultantId);
                if (items == null)
                    return this.ApiNotFoundRequest();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType((200), Type = typeof(RatingVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Post([FromBody] RatingCreateVO item)
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
        [ProducesResponseType((200), Type = typeof(RatingVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Put([FromBody] RatingEditVO item)
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
