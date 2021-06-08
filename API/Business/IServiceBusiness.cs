using API.Data.VO;
using API.Hypermedia.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Business
{
    public interface IServiceBusiness
    {
        Task<ServiceVO> CreateAsync(ServiceCreateVO service);
        //StepVO Update(StepVO step);
        Task<ServiceVO> FindByIdAsync(long id);
        //List<StepVO> FindByType(string type);
        Task<List<ServiceVO>> FindAllAsync();
        //PagedSearchVO<StepVO> FindWithPagedSearch(
        //    string name, string sortDirection, int pageSize, int page);
        Task DeleteAsync(long id);
    }
}
