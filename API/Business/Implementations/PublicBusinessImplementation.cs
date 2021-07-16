using API.Data.Converter.Implementations;
using API.Data.VO;
using API.Hypermedia.Utils;
using Database.Model;
using Database.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class PublicBusinessImplementation : IPublicBusiness
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ServiceConverter _converter;

        public PublicBusinessImplementation(IServiceRepository serviceRepository, FileConverter fileConverter)
        {
            _serviceRepository = serviceRepository;
            _converter = new ServiceConverter(fileConverter);
        }

        public async Task<PagedSearchVO<ServiceVO>> FindWithPagedSearchAsync(string name, string sortDirection, int pageSize, int page, CancellationToken cancellationToken)
        {
            var result = await _serviceRepository
                .FindByServiceOrConsultantNameAsync(
                    name, 
                    sortDirection, 
                    pageSize, 
                    page, 
                    cancellationToken,
                    nameof(Service.Picture),
                    $"{nameof(Service.User)}.{nameof(User.ProfilePicture)}");

            return new PagedSearchVO<ServiceVO>
            {
                CurrentPage = result.CurrentPage,
                List = _converter.Parse(result.Items),
                PageSize = result.PageSize,
                TotalResults = result.TotalResults
            };
        }
    }
}
