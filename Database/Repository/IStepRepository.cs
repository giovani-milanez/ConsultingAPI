using Database.Model;
using Database.Utils;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IStepRepository : IRepository<Step>
    {
        Task<PagedSearch<Step>> FindWithPagedSearchAsync(string type, string sortDirection, int pageSize, int page);
    }
}
