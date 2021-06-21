using Database.Model;
using Database.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IStepRepository : IRepository<Step>
    {
        Task<List<Step>> FindAllAsync(User requester, params string[] includes);
        Task<PagedSearch<Step>> FindWithPagedSearchAsync(
            User requester,
            PagedRequest paging,
            CancellationToken cancellationToken,
            params string[] includes);
        Task<bool> ExistsAsync(string type);
    }
}
