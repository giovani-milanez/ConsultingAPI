using Database.Model;
using Database.Model.Context;
using Database.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context) : base(context)
        {
        }

        public Task<User> ValidateCredentialsAsync(string email, string password)
        {
            var pass = ComputeHash(password, new SHA256CryptoServiceProvider());
            return _context.Users.Include(nameof(User.ProfilePicture)).FirstOrDefaultAsync(u => (u.Email == email) && (u.Password == pass));
        }

        public Task<User> FindByEmailAsync(string email)
        {
            return _context.Users.Include(nameof(User.ProfilePicture)).SingleOrDefaultAsync(u => (u.Email == email));
        }

        public Task<User> FindByIdAsync(int id)
        {
            return _context.Users.SingleOrDefaultAsync(u => (u.Id == id));
        }

        public async Task<bool> RevokeTokenAsync(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => (u.Email == email));
            if (user == null) return false;
            user.RefreshToken = null;
            _context.SaveChanges();
            return true;
        }

        public async Task<User> RefreshUserInfoAsync(User user)
        {
            if (!_context.Users.Any(u => u.Id.Equals(user.Id))) return null;

            var result = await _context.Users.SingleOrDefaultAsync(p => p.Id.Equals(user.Id));
            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await this._context.Users.AnyAsync(p => p.Email.ToUpper().Equals(email.ToUpper()));
        }

        private string ComputeHash(string input, SHA256CryptoServiceProvider algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            return BitConverter.ToString(hashedBytes);
        }

        public Task<User> FindByExternalProviderAsync(string provider, string providerUserId)
        {
            return FindByEmailAsync(providerUserId);
        }

        public async Task<User> AutoProvisionUserAsync(string provider, string providerUserId, List<Claim> claims)
        {
            var newUser = new User
            {
                Name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value,
                Type = "client",
                Email = providerUserId,
                CreatedAt = DateTime.UtcNow,
                IsEmailConfirmed = true
            };
            newUser = await this.CreateAsync(newUser);
            return newUser;
        }
    }
}
