using API.Data.Converter.Implementations;
using API.Data.VO;
using Database.Model;
using Database.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class ServiceBusinessImplementation : IServiceBusiness
    {
        private readonly IRepository<Service> _repository;
        private readonly ServiceConverter _converter;

        public ServiceBusinessImplementation(IRepository<Service> repository)
        {
            _repository = repository;
            _converter = new ServiceConverter();
        }

        public async Task<ServiceVO> CreateAsync(ServiceCreateVO vo)
        {
            var entity = _converter.Parse(vo);
            entity = await _repository.CreateAsync(entity);
            return await this.FindByIdAsync(entity.Id);
        }

        public Task DeleteAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        public async Task<List<ServiceVO>> FindAllAsync()
        {
            return _converter.Parse(
                await _repository.FindAllAsync(
                    nameof(Service.ServicesSteps),
                    $"{nameof(Service.ServicesSteps)}.{nameof(ServicesStep.Step)}",
                    nameof(Service.User)
                )
            );
        }

        public async Task<ServiceVO> FindByIdAsync(long id)
        {
            return _converter.Parse(
                await _repository.FindByIdAsync(id,
                    nameof(Service.ServicesSteps),
                    $"{nameof(Service.ServicesSteps)}.{nameof(ServicesStep.Step)}",
                    nameof(Service.User)
                )
           );
        }
    }
}
