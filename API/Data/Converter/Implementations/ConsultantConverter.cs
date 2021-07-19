using API.Data.Converter.Contract;
using API.Data.VO.User;
using Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class ConsultantConverter : IParser<User, ConsultantVO>
    {
        private FileConverter FileConverter { get; set; }
        private readonly ServiceShortConverter ServiceConverter;
        public ConsultantConverter(FileConverter fileConverter)
        {
            FileConverter = fileConverter;
            ServiceConverter = new ServiceShortConverter(fileConverter);
        }

        public ConsultantVO Parse(User origin)
        {
            if (origin == null) return null;
            return new ConsultantVO
            {
                Id = origin.Id,
                Name = origin.Name,
                RateMeanStars = origin.RateMeanStars,
                RateCount = origin.RateCount,
                ProfilePicUrl = origin.ProfilePicture != null ? FileConverter.Parse(origin.ProfilePicture).Url : "",
                Gender = origin.Gender,
                ShortDescription = origin.ShortDescription,
                LongDescription = origin.LongDescription,
                CreatedAt = origin.CreatedAt,
                Services = ServiceConverter.Parse(origin.Services.ToList())
            };
        }

        public List<ConsultantVO> Parse(List<User> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
