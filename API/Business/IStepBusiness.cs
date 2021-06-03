using API.Data.VO;
using API.Hypermedia.Utils;
using System.Collections.Generic;

namespace API.Business
{
    public interface IStepBusiness
    {
        StepVO Create(StepVO step);
        StepVO Update(StepVO step);
        StepVO FindById(long id);
        List<StepVO> FindByType(string type);
        List<StepVO> FindAll();
        PagedSearchVO<StepVO> FindWithPagedSearch(
            string name, string sortDirection, int pageSize, int page);
        void Delete(long id);
    }
}
