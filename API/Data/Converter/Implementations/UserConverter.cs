using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Extension;
using Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class UserConverter : IParser<User, UserShortVO>
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
                RateMeanStars = origin.RateMeanStars,
                RateCount = origin.RateCount,
                ProfilePicUrl = origin.ProfilePicture != null ? FileConverter.Parse(origin.ProfilePicture).Url : ""
            };
        }

        public List<UserShortVO> Parse(List<User> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
