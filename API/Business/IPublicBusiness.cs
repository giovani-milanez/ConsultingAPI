using API.Data.VO;
using API.Data.VO.User;
using API.Hypermedia.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace API.Business
{
    public interface IPublicBusiness
    {
        Task<PagedSearchVO<ServiceVO>> FindWithPagedSearchAsync(
            string name, string sortDirection, int pageSize, int page, CancellationToken cancellationToken);
        Task<ConsultantVO> GetConsultantByIdAsync(int id);
        Task<ServiceVO> GetServiceByIdAsync(int id);
    }
}
