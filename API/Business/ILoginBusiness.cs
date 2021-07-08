using API.Data.VO;
using API.Data.VO.Token;
using System.Threading.Tasks;

namespace API.Business
{
    public interface ILoginBusiness
    {
        Task<TokenVO> ValidateCredentialsAsync(UserLoginVO user);
        Task<TokenVO> ValidateCredentialsAsync(RefreshTokenVO token);
        Task<bool> RevokeTokenAsync(string email);
        Task<TokenVO> RegisterUserAsync(UserRegisterVO user);
        Task<TokenVO> RegisterGoogleUserAsync(GoogleTokenRegisterVO token);
        Task<TokenVO> RegisterFacebookUserAsync(FacebookTokenRegisterVO token);
        Task<TokenVO> ValidateGoogleUserAsync(string jwtIdToken);
        Task<TokenVO> ValidateFacebookUserAsync(string aaccessToken);
    }
}
