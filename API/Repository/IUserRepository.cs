using API.Data.VO;
using API.Model;

namespace API.Repository
{
    public interface IUserRepository
    {
        User ValidateCredentials(UserVO user);
        User ValidateCredentials(string username);
        User RefreshUserInfo(User user);

        bool RevokeToken(string username);
    }
}
