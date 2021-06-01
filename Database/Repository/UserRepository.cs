using Database.Model;
using Database.Model.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MySQLContext _context;

        public UserRepository(MySQLContext context)
        {
            _context = context;
        }

        public Task<User> ValidateCredentialsAsync(string email, string password)
        {
            var pass = ComputeHash(password, new SHA256CryptoServiceProvider());
            return _context.Users.FirstOrDefaultAsync(u => (u.Email == email) && (u.Password == pass));
        }

        public Task<User> FindByEmailAsync(string email)
        {
            return _context.Users.SingleOrDefaultAsync(u => (u.Email == email));
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

        private string ComputeHash(string input, SHA256CryptoServiceProvider algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            return BitConverter.ToString(hashedBytes);
        }
    }
}
