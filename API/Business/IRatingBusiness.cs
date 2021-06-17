using API.Data.VO.Rating;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Business
{
    public interface IRatingBusiness
    {
        Task<RatingVO> CreateAsync(RatingCreateVO vo);
        Task<RatingVO> UpdateAsync(RatingEditVO vo);
        Task<RatingVO> FindByIdAsync(long id);
        Task<List<RatingVO>> FindAllByConsultantIdAsync(long consultantId);
        Task DeleteAsync(long id);
    }
}
