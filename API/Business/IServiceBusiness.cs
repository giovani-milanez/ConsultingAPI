using API.Data.VO;
using API.Hypermedia.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Business
{
    public interface IServiceBusiness
    {
        Task<ServiceVO> CreateAsync(ServiceCreateVO service);
        Task<ServiceVO> UpdateAsync(ServiceEditVO service);
        Task<ServiceVO> FindByIdAsync(long id);
        Task<List<ServiceVO>> FindAllAsync();
        Task<PagedSearchVO<ServiceVO>> FindWithPagedSearchAsync(
            string title, string sortDirection, int pageSize, int page, CancellationToken cancellationToken);
        Task DeleteAsync(long id);
    }
}
