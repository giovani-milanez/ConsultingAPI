using Database.Model.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> CreateAsync(T item);
        Task<T> FindByIdAsync(long id, bool tracking, params string[] includes);
        Task<List<T>> FindAllAsync(bool tracking, params string[] includes);
        Task<T> UpdateAsync(T item);
        Task<T> UpdateTrackedAsync(T trackedItem, T newValues);
        //Task DeleteAsync(long id);
        Task DeleteTrackedAsync(T trackedItem);
        Task<bool> ExistsAsync(long id);
        //Task<List<T>> FindWithPagedSearchAsync(string query, params string[] includes);
        //Task<int> GetCountAsync(string query);
    }
}
