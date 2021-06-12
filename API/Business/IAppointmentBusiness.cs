using API.Data.VO;
using API.Hypermedia.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Business
{
    public interface IAppointmentBusiness
    {
        Task<AppointmentVO> CreateAsync(AppointmentCreateVO service);
        Task<AppointmentStepVO> SubmitStep(AppointmentStepSubmitVO step);
        Task<AppointmentStepVO> EditStep(AppointmentStepSubmitVO step);

        Task<AppointmentVO> FindByIdAsync(long id);
        Task<List<AppointmentVO>> FindAllAsync();
        Task<PagedSearchVO<AppointmentVO>> FindWithPagedSearchAsync(
            string clientName, string sortDirection, int pageSize, int page);
        Task DeleteAsync(long id);
    }
}
