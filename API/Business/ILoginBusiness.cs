using API.Data.VO;

namespace API.Business
{
    public interface ILoginBusiness
    {
        TokenVO ValidateCredentials(UserVO user);
        TokenVO ValidateCredentials(TokenVO token);
        public bool RevokeToken(string email);
    }
}
