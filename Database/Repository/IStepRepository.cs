using Database.Model;
using Database.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IStepRepository : IRepository<Step>
    {
        Task<List<Step>> FindAllAsync(User requester, params string[] includes);
        Task<PagedSearch<Step>> FindWithPagedSearchAsync(string type, User requester, string sortDirection, int pageSize, int page);
        Task<bool> ExistsAsync(string type);
    }
}
