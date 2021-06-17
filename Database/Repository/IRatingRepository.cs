using Database.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task<bool> ExistsByAppointmentIdAsync(long appointmentId);
        Task<Rating> AddAndUpdateConsultantRatingAsync(Rating item, long consultantId);
        Task<Rating> EditAndUpdateConsultantRatingAsync(Rating item, long consultantId, int previousStar);
        Task<List<Rating>> FindAllByConsultantIdAsync(long consultantId, params string[] includes);

    }
}
