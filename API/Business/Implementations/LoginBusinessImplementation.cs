using API.Configurations;
using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Data.VO.Token;
using API.Exceptions;
using API.Services;
using Database.Model;
using Database.Repository;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class LoginBusinessImplementation : ILoginBusiness
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private TokenConfigurations _configuration;

        private IUserRepository _repository;
        private readonly IFileRepository _fileRepository;
        private readonly ITokenService _tokenService;
        private readonly UserConverter _userConverter;

        public LoginBusinessImplementation(TokenConfigurations configuration, IUserRepository repository, IFileRepository fileRepository, ITokenService tokenService, FileConverter fileConverter)
        {
            _configuration = configuration;
            _repository = repository;
            _fileRepository = fileRepository;
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

        public async Task<TokenVO> ValidateCredentialsAsync(RefreshTokenVO token)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(token.AccessToken);

            var email = principal.FindFirst(ClaimTypes.Email);
            if (email == null)
                return null;

            var user = await _repository.FindByEmailAsync(email.Value);

            if (user == null ||
                user.RefreshToken != token.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now) return null;

            var newToken = await GetTokenFromUserAsync(user);
            user.RefreshToken = newToken.RefreshToken;

            await _repository.RefreshUserInfoAsync(user);            

            return newToken;
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
                throw new FieldValidationException("Email já cadastrado, use um diferente.", new Data.FieldError(nameof(vo.Email), "email already taken"));
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

        public async Task<TokenVO> RegisterGoogleUserAsync(GoogleTokenRegisterVO tokenVO)
        {
            var validPayload = await GoogleJsonWebSignature.ValidateAsync(tokenVO.JwtIdToken);
            if (validPayload == null || !validPayload.EmailVerified)
                return null;

            bool emailExists = await _repository.EmailExistsAsync(validPayload.Email);
            if (emailExists)
            {
                throw new FieldValidationException("Email já cadastrado, use um diferente.", new Data.FieldError(nameof(validPayload.Email), "email already taken"));
            }

            var entity = new User
            {
                Name = validPayload.Name,
                Type = tokenVO.IsConsultant ? "consultant" : "client",
                Email = validPayload.Email,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                LoginProvider = "Google"
            };
            entity = await _repository.CreateAsync(entity);

            if (!String.IsNullOrWhiteSpace(validPayload.Picture))
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(validPayload.Picture);

                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    var guid = Guid.NewGuid().ToByteArray();
                    var fileType = response.Content.Headers.ContentType?.MediaType;
                    fileType = fileType.Replace("image/", ".");
                    FileDetail file = new FileDetail
                    {
                        Guid = guid,
                        Name = String.Concat(validPayload.Name, fileType),
                        Type = fileType,
                        Size = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : 0,
                        UploaderId = entity.Id,
                        Content = new FileContent { FileGuid = guid, Content = bytes }
                    };
                    file = await _fileRepository.CreateAsync(file);

                    // update users profile pic
                    entity.ProfilePictureId = file.Id;
                    await _repository.UpdateAsync(entity);
                }
            }

            return await GetTokenFromUserAsync(entity);
        }

        public async Task<TokenVO> RegisterFacebookUserAsync(FacebookTokenRegisterVO tokenVo)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://graph.facebook.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync($"debug_token?input_token={tokenVo.AccessToken}&access_token={tokenVo.AccessToken}");
            bool isValid = false;
            if (response.IsSuccessStatusCode)
            {
                var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
                var data = payload.GetProperty("data");
                isValid = data.GetProperty("is_valid").GetBoolean();
            }

            if (!isValid) return null;

            response = await client.GetAsync($"me?fields=name,email,id&access_token={tokenVo.AccessToken}");
            if (!response.IsSuccessStatusCode) return null;
            var payloadInfo = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

            var email = payloadInfo.GetProperty("email").GetString();
            var name = payloadInfo.GetProperty("name").GetString();
            var id = payloadInfo.GetProperty("id").GetString();

            bool emailExists = await _repository.EmailExistsAsync(email);
            if (emailExists)
            {
                throw new FieldValidationException("Email já cadastrado, use um diferente.", new Data.FieldError("Email", "email already taken"));
            }

            var entity = new User
            {
                Name = name,
                Type = tokenVo.IsConsultant ? "consultant" : "client",
                Email = email,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                LoginProvider = "Facebook"
            };
            entity = await _repository.CreateAsync(entity);

            response = await client.GetAsync($"v11.0/{id}/picture");
            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var guid = Guid.NewGuid().ToByteArray();
                var fileType = response.Content.Headers.ContentType?.MediaType;
                fileType = fileType.Replace("image/", ".");
                FileDetail file = new FileDetail
                {
                    Guid = guid,
                    Name = String.Concat(name, fileType),
                    Type = fileType,
                    Size = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : 0,
                    UploaderId = entity.Id,
                    Content = new FileContent { FileGuid = guid, Content = bytes }
                };
                file = await _fileRepository.CreateAsync(file);

                // update users profile pic
                entity.ProfilePictureId = file.Id;
                await _repository.UpdateAsync(entity);
            }

            return await GetTokenFromUserAsync(entity);

        }
    }
}
