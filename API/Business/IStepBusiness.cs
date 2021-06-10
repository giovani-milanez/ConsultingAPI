using API.Data.VO;
using API.Hypermedia.Utils;
using Database.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Business
{
    public interface IStepBusiness
    {
        Task<StepVO> CreateAsync(StepCreateVO step);
        Task<StepVO> UpdateAsync(StepEditVO step);
        Task<StepVO> FindByIdAsync(long id);
        Task<List<StepVO>> FindByTypeAsync(string type);
        Task<List<StepVO>> FindAllAsync();
        Task<PagedSearchVO<StepVO>> FindWithPagedSearchAsync(
            string name, string sortDirection, int pageSize, int page);
        Task DeleteAsync(long id);
    }
}
