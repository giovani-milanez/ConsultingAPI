using API.Configurations;
using API.Data.VO;
using Database.Repository;
using API.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class LoginBusinessImplementation : ILoginBusiness
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private TokenConfigurations _configurations;

        private IUserRepository _repository;
        private readonly ITokenService _tokenService;

        public LoginBusinessImplementation(TokenConfigurations configurations, IUserRepository repository, ITokenService tokenService)
        {
            _configurations = configurations;
            _repository = repository;
            _tokenService = tokenService;
        }

        public async Task<TokenVO> ValidateCredentialsAsync(UserLoginVO userCredentials)
        {
            var user = await _repository.ValidateCredentialsAsync(userCredentials.Email, userCredentials.Password);
            if (user == null) return null;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Email)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configurations.DaysToExpiry);

            await _repository.RefreshUserInfoAsync(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configurations.Minutes);
            
            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }

        public async Task<TokenVO> ValidateCredentialsAsync(TokenVO token)
        {
            var accessToken = token.AccessToken;
            var refreshToken = token.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            
            var email = principal.Identity.Name;
            
            var user = await _repository.FindByEmailAsync(email);
            if (user == null || 
                user.RefreshToken != refreshToken || 
                user.RefreshTokenExpiryTime <= DateTime.Now) return null;

            accessToken = _tokenService.GenerateAccessToken(principal.Claims);
            refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            await _repository.RefreshUserInfoAsync(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configurations.Minutes);

            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }

        public Task<bool> RevokeTokenAsync(string email)
        {
            return _repository.RevokeTokenAsync(email);
        }

        public void RegisterNewUser(UserSignupVO user)
        {
            throw new NotImplementedException();
        }
    }
}
