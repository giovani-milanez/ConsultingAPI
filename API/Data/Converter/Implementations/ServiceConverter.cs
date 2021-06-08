using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class ServiceConverter : IParser<ServiceCreateVO, Service>, IParser<Service, ServiceVO>
    {
        private readonly ServicesStepConverter ServicesStepConverter;
        private readonly UserConverter UserConverter;

        public ServiceConverter()
        {
            ServicesStepConverter = new ServicesStepConverter();
            UserConverter = new UserConverter();
        }

        public Service Parse(ServiceCreateVO origin)
        {
            if (origin == null) return null;
            return new Service
            {
              UserId = origin.UserId,
              Title = origin.Title,
              Description = origin.Description,
              IsDeleted = false,
              IsGlobal = false,
              ServicesSteps = ServicesStepConverter.Parse(origin.Steps.ToList())
            };
        }

        public ServiceVO Parse(Service origin)
        {
            if (origin == null) return null;
            return new ServiceVO
            {
                Id = origin.Id,
                User = UserConverter.Parse(origin.User),
                Title = origin.Title,
                Description = origin.Description,
                IsDeleted = origin.IsDeleted,
                IsGlobal = origin.IsGlobal,
                Steps = ServicesStepConverter.Parse(origin.ServicesSteps.ToList())
            };
        }

        public List<ServiceVO> Parse(List<Service> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }


        public List<Service> Parse(List<ServiceCreateVO> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
