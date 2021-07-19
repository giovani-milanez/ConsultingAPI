using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class ServiceShortConverter : IParser<Service, ServiceShortVO>
    {
        private readonly FileConverter FileConverter;
        public ServiceShortConverter(FileConverter fileConverter)
        {
            FileConverter = fileConverter;
        }

        public ServiceShortVO Parse(Service origin)
        {
            if (origin == null) return null;
            return new ServiceShortVO
            {
                Id = origin.Id,
                Title = origin.Title,
                Description = origin.Description,
                IsDeleted = origin.IsDeleted,
                IsGlobal = origin.IsGlobal,
                PictureUrl = origin.Picture != null ? FileConverter.Parse(origin.Picture).Url : ""
            };
        }

        public List<ServiceShortVO> Parse(List<Service> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
