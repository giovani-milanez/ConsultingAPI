using API.Configurations;
using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Exceptions;
using API.Services;
using Database.Model;
using Database.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class LoginBusinessImplementation : ILoginBusiness
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private TokenConfigurations _configuration;

        private IUserRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly UserConverter _userConverter;

        public LoginBusinessImplementation(TokenConfigurations configuration, IUserRepository repository, ITokenService tokenService, FileConverter fileConverter)
        {
            _configuration = configuration;
            _repository = repository;
            _tokenService = tokenService;
            _userConverter = new UserConverter(fileConverter);
        }

        public async Task<TokenVO> ValidateCredentialsAsync(UserLoginVO userCredentials)
        {
            if (String.IsNullOrWhiteSpace(userCredentials.Email) || String.IsNullOrWhiteSpace(userCredentials.Password)) return null;

            var user = await _repository.ValidateCredentialsAsync(userCredentials.Email, userCredentials.Password);
            if (user == null) return null;
            return await GetTokenFromUserAsync(user);
        }

        public async Task<TokenVO> ValidateCredentialsAsync(TokenVO token)
        {
            var accessToken = token.AccessToken;
            var refreshToken = token.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

            var email = principal.FindFirst(ClaimTypes.Email);
            if (email == null)
                return null;

            var user = await _repository.FindByEmailAsync(email.Value);

            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now) return null;

            accessToken = _tokenService.GenerateAccessToken(principal.Claims);
            refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            await _repository.RefreshUserInfoAsync(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }
        public async Task<bool> RevokeTokenAsync(string email)
        {
            return await _repository.RevokeTokenAsync(email);
        }

        public async Task<TokenVO> RegisterUserAsync(UserRegisterVO vo)
        {
            if (String.IsNullOrWhiteSpace(vo.Name) || vo.Name.Length < 3)
            {
                throw new FieldValidationException("name too short", new Data.FieldError(nameof(vo.Name), "name too short"));
            }
            if (String.IsNullOrWhiteSpace(vo.Password) || vo.Password.Length < 6)
            {
                throw new FieldValidationException("password too short", new Data.FieldError(nameof(vo.Password), "password too short"));
            }
            if (vo.Password.Length > 50)
            {
                throw new FieldValidationException("password too long", new Data.FieldError(nameof(vo.Password), "password too long"));
            }
            if (!IsValidEmail(vo.Email))
            {
                throw new FieldValidationException("invalid email address", new Data.FieldError(nameof(vo.Email), "invalid email address"));
            }
            bool emailExists = await _repository.EmailExistsAsync(vo.Email);
            if (emailExists)
            {
                throw new FieldValidationException("email already taken", new Data.FieldError(nameof(vo.Email), "email already taken"));
            }

            var entity = _userConverter.Parse(vo);
            entity = await _repository.CreateAsync(entity);
            return await GetTokenFromUserAsync(entity);
        }

        private bool IsValidEmail(string email)
        {
            var foo = new EmailAddressAttribute();
            return foo.IsValid(email);
        }

        private async Task<TokenVO> GetTokenFromUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Type),
                new Claim("name", user.Name)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.DaysToExpiry);

            await _repository.RefreshUserInfoAsync(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }
    }
}
