using API.Data.VO;
using System.Threading.Tasks;

namespace API.Business
{
    public interface ILoginBusiness
    {
        Task<TokenVO> ValidateCredentialsAsync(UserLoginVO user);
        Task<TokenVO> ValidateCredentialsAsync(RefreshTokenVO token);
        Task<bool> RevokeTokenAsync(string email);
        Task<TokenVO> RegisterUserAsync(UserRegisterVO user);
    }
}
