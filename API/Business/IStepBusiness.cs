using API.Data.VO;
using API.Hypermedia.Utils;
using Database.Model;
using Database.Utils;
using System.Collections.Generic;
using System.Threading;
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
        Task<PagedSearchVO<StepVO>> FindWithPagedSearchAsync(PagedRequest paging, CancellationToken cancellationToken);
        Task DeleteAsync(long id);
    }
}
