using Database.Model;
using Database.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<AppointmentStep> SubmitStep(Appointment appointment, long StepId, JsonDocument SubmitData);
        Task<AppointmentStep> EditStep(Appointment appointment, long StepId, JsonDocument SubmitData);
        Task<List<Appointment>> FindAllAsync(User requester, params string[] includes);
        Task<PagedSearch<Appointment>> FindWithPagedSearchAsync(
            string clientName, 
            User requester, 
            string sortDirection, 
            int pageSize, 
            int page,
            CancellationToken cancellationToken,
            params string[] includes);
    }
}
