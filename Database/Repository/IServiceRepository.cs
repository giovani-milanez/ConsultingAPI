using Database.Model;
using Database.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task<Service> UpdateManuallyAsync(Service item);
        Task<List<Service>> FindAllAsync(User requester, params string[] includes);
        Task<PagedSearch<Service>> FindWithPagedSearchAsync(
            string title, 
            User requester, 
            string sortDirection, 
            int pageSize, 
            int page,
            CancellationToken cancellationToken,
            params string[] includes);

        Task<PagedSearch<Service>> FindByServiceOrConsultantNameAsync(
            string serviceOrConsultantName,
            string sortDirection,
            int pageSize,
            int page,
            CancellationToken cancellationToken,
            params string[] includes);
    }
}
