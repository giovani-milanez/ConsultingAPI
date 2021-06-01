using Database.Model;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IUserRepository
    {
        Task<User> ValidateCredentialsAsync(string email, string password);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByIdAsync(int id);
        Task<User> RefreshUserInfoAsync(User user);

        Task<bool> RevokeTokenAsync(string username);
    }
}
