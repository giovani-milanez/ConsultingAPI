using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Extension;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace API.Data.Converter.Implementations
{
    public class UserConverter : IParser<User, UserShortVO>, IParser<UserRegisterVO, User>
    {
        private FileConverter FileConverter { get; set; }

        public UserConverter(FileConverter fileConverter)
        {
            FileConverter = fileConverter;
        }

        public UserShortVO Parse(User origin)
        {
            if (origin == null) return null;
            return new UserShortVO
            {
                Id = origin.Id,
                IsConsultant = origin.IsConsultant(),
                Name = origin.Name,
                Email = origin.Email,
                //IsEmailConfirmed = origin.IsEmailConfirmed,
                RateMeanStars = origin.RateMeanStars,
                RateCount = origin.RateCount,
                ShortDescription = origin.ShortDescription,
                ProfilePicUrl = origin.ProfilePicture != null ? FileConverter.Parse(origin.ProfilePicture).Url : ""
            };
        }

        public User Parse(UserRegisterVO origin)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(origin.Password);
            Byte[] hashedBytes = new SHA256CryptoServiceProvider().ComputeHash(inputBytes);

            if (origin == null) return null;
            return new User
            {
                Name = origin.Name,
                Type = origin.IsConsultant ? "consultant" : "client",
                Email = origin.Email,
                Password = BitConverter.ToString(hashedBytes),
                IsEmailConfirmed = true,
                //EmailConfirmationCode = Guid.NewGuid().ToByteArray(),
                CreatedAt = DateTime.UtcNow
            };
        }

        public List<UserShortVO> Parse(List<User> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }

        public List<User> Parse(List<UserRegisterVO> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
