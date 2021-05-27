using API.Data.VO;
using API.Model;
using API.Model.Context;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MySQLContext _context;

        public UserRepository(MySQLContext context)
        {
            _context = context;
        }

        public User ValidateCredentials(UserVO user)
        {
            var pass = ComputeHash(user.Password, new SHA256CryptoServiceProvider());
            return _context.Users.FirstOrDefault(u => (u.Email == user.Email) && (u.Password == pass));
        }

        public User ValidateCredentials(string email)
        {
            return _context.Users.SingleOrDefault(u => (u.Email == email));
        }

        public bool RevokeToken(string email)
        {
            var user = _context.Users.SingleOrDefault(u => (u.Email == email));
            if (user == null) return false;
            user.RefreshToken = null;
            _context.SaveChanges();
            return true;
        }

        public User RefreshUserInfo(User user)
        {
            if (!_context.Users.Any(u => u.Id.Equals(user.Id))) return null;

            var result = _context.Users.SingleOrDefault(p => p.Id.Equals(user.Id));
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
