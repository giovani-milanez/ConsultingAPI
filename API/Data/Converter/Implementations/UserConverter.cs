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
        public UserShortVO Parse(User origin)
        {
            if (origin == null) return null;
            return new UserShortVO
            {
                Id = origin.Id,
                IsConsultant = origin.IsConsultant(),
                Name = origin.Name,
                Email = origin.Email
            };
        }

        public List<UserShortVO> Parse(List<User> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
