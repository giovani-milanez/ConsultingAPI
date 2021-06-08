using API.Business;
using API.Data.VO;
using API.Hypermedia.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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

        [HttpGet("{sortDirection}/{pageSize}/{page}")]
        [ProducesResponseType((200), Type = typeof(List<StepVO>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Get(
           [FromQuery] string type,
           string sortDirection,
           int pageSize,
           int page
           )
        {
            return Ok(await _business.FindWithPagedSearchAsync(type, sortDirection, pageSize, page));
        }

        [HttpGet("{id}")]
        [ProducesResponseType((200), Type = typeof(StepVO))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> GetAsync(long id)
        {
            var item = await _business.FindByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType((200), Type = typeof(StepVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Post([FromBody] StepVO item)
        {
            if (item == null) return BadRequest();
            return Ok(await _business.CreateAsync(item));
        }

        [HttpPut]
        [ProducesResponseType((200), Type = typeof(StepVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> Put([FromBody] StepVO item)
        {
            if (item == null) return BadRequest();
            return Ok(await _business.UpdateAsync(item));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            await _business.DeleteAsync(id);
            return NoContent();
        }
    }
}
