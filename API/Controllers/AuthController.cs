using API.Business;
using API.Data.VO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ILoginBusiness _loginBussiness;

        public AuthController(ILoginBusiness loginBussiness)
        {
            _loginBussiness = loginBussiness;
        }

        [HttpPost]
        [Route("signin")]
        public IActionResult Signin([FromBody] UserLoginVO user)
        {
            if (user == null) return BadRequest("Invalid client request");
            var token = _loginBussiness.ValidateCredentialsAsync(user);
            if (token == null) return Unauthorized();
            return Ok(token);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] TokenVO tokenVo)
        {
            if (tokenVo == null) return BadRequest("Invalid client request");
            var token = await _loginBussiness.ValidateCredentialsAsync(tokenVo);
            if (token == null) return BadRequest("Invalid client request");
            return Ok(token);
        }

        [HttpGet]
        [Route("revoke")]
        [Authorize("Bearer")]
        public async Task<IActionResult> RevokeAsync()
        {
            var username = User.Identity.Name;
            var result = await _loginBussiness.RevokeTokenAsync(username);
            if (!result) return BadRequest("Invalid client request");
            return NoContent();
        }
    }
}
