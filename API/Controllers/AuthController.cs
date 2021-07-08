using API.Business;
using API.Data.VO;
using API.Data.VO.Token;
using API.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        [Route("signup/google")]
        [ProducesResponseType((200), Type = typeof(TokenVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SignupGoogleAsync([FromBody] GoogleTokenRegisterVO tokenVo)
        {
            try
            {
                var token = await _loginBusiness.RegisterGoogleUserAsync(tokenVo);
                if (token == null) return Unauthorized();

                return Ok(token);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpPost]
        [Route("signup/facebook")]
        [ProducesResponseType((200), Type = typeof(TokenVO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SignupFacebookAsync([FromBody] FacebookTokenRegisterVO tokenVo)
        {
            try
            {
                var token = await _loginBusiness.RegisterFacebookUserAsync(tokenVo);
                if (token == null) return Unauthorized();

                return Ok(token);
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
            try
            {
                var token = await _loginBusiness.ValidateCredentialsAsync(user);
                if (token == null) return Unauthorized();
                return Ok(token);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpPost]
        [Route("signin/google")]
        [ProducesResponseType((200), Type = typeof(TokenVO))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SigninGoogleAsync([FromBody] GoogleTokenLoginVO tokenVo)
        {
            if (tokenVo == null || String.IsNullOrWhiteSpace(tokenVo.JwtIdToken)) return BadRequest("Ivalid client request");
            try
            {
                var token = await _loginBusiness.ValidateGoogleUserAsync(tokenVo.JwtIdToken);
                if (token == null) return Unauthorized();
                return Ok(token);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpPost]
        [Route("signin/facebook")]
        [ProducesResponseType((200), Type = typeof(TokenVO))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SigninFacebookAsync([FromBody] FacebookTokenLoginVO tokenVo)
        {
            if (tokenVo == null || String.IsNullOrWhiteSpace(tokenVo.AccessToken)) return BadRequest("Ivalid client request");
            try
            {
                var token = await _loginBusiness.ValidateFacebookUserAsync(tokenVo.AccessToken);
                if (token == null) return Unauthorized();
                return Ok(token);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType((200), Type = typeof(TokenVO))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenVO tokenVo)
        {
            if (tokenVo == null) return BadRequest("Ivalid client request");
            try
            {
                var token = await _loginBusiness.ValidateCredentialsAsync(tokenVo);
                if (token == null) return Unauthorized();
                return Ok(token);
            }
            catch (Exception ex)
            {
                return this.ApiResulFromException(ex);
            }
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
