using API.Business;
using API.Data.VO;
using API.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ILoginBusiness _loginBusiness;

        public AuthController(ILoginBusiness loginBusiness)
        {
            _loginBusiness = loginBusiness;
        }

        [HttpPost]
        [Route("signup")]
        [ProducesResponseType((200), Type = typeof(TokenVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SignupAsync([FromBody] UserRegisterVO user)
        {
            try
            {
                var item = await _loginBusiness.RegisterUserAsync(user);
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
        [Route("signin")]
        [ProducesResponseType((200), Type = typeof(TokenVO))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SigninAsync([FromBody] UserLoginVO user)
        {
            if (user == null) return BadRequest("Ivalid client request");
            var token = await _loginBusiness.ValidateCredentialsAsync(user);
            if (token == null) return Unauthorized();
            return Ok(token);
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType((200), Type = typeof(TokenVO))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RefreshAsync([FromBody] TokenVO tokenVo)
        {
            if (tokenVo is null) return BadRequest("Ivalid client request");
            var token = await _loginBusiness.ValidateCredentialsAsync(tokenVo);
            if (token == null) return BadRequest("Ivalid client request");
            return Ok(token);
        }


        [HttpGet]
        [Route("revoke")]
        [Authorize("Bearer")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RevokeAsync()
        {
            var username = User.Identity.Name;
            var result = await _loginBusiness.RevokeTokenAsync(username);

            if (!result) return BadRequest("Ivalid client request");
            return NoContent();
        }
    }
}
