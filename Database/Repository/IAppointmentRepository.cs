using Database.Model;
using Database.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<List<Appointment>> FindAllAsync(User requester, params string[] includes);
        Task<PagedSearch<Appointment>> FindWithPagedSearchAsync(string clientName, User requester, string sortDirection, int pageSize, int page);
    }
}
