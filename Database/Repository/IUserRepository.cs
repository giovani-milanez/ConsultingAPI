using Database.Model;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> ValidateCredentialsAsync(string email, string password);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByIdAsync(int id);
        Task<User> RefreshUserInfoAsync(User user);

        Task<bool> EmailExistsAsync(string email);
        Task<bool> RevokeTokenAsync(string username);
        Task<User> FindByExternalProviderAsync(string provider, string providerUserId);
        Task<User> AutoProvisionUserAsync(string provider, string providerUserId, List<Claim> claims);
    }
}
