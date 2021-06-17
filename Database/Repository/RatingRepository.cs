using Database.Model;
using Database.Model.Context;
using Database.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Repository
{
    public class RatingRepository : GenericRepository<Rating>, IRatingRepository
    {
        public RatingRepository(DatabaseContext context) : base(context)
        {
        }

        public Task<bool> ExistsByAppointmentIdAsync(long appointmentId)
        {
            return this._context.Ratings.AnyAsync(x => x.AppointmentId == appointmentId);
        }

        public async Task<Rating> AddAndUpdateConsultantRatingAsync(Rating item, long consultantId)
        {
            // create rating
            var newItem = await CreateAsync(item);

            var consultant = await this._context.Users.Where(x => x.Id == consultantId).SingleOrDefaultAsync();
            if (consultant == null)
            {
                throw new Exception($"can't find consultant of id {consultantId}");
            }

            // update consultant's rating mean
            var previousCount = consultant.RateCount;
            var previousMean = consultant.RateMeanStars;
            consultant.RateMeanStars = ((previousMean * previousCount) + item.Stars) / (previousCount + 1);
            consultant.RateCount++;
            await _context.SaveChangesAsync();

            return newItem;
        }

        public async Task<Rating> EditAndUpdateConsultantRatingAsync(Rating item, long consultantId, int previousStar)
        {
            // update rating
            var newItem = await UpdateAsync(item);

            var consultant = await this._context.Users.Where(x => x.Id == consultantId).SingleOrDefaultAsync();
            if (consultant == null)
            {
                throw new Exception($"can't find consultant of id {consultantId}");
            }

            // update consultant's rating mean
            var newMean =
                ((consultant.RateMeanStars * consultant.RateCount) - previousStar + item.Stars) / consultant.RateCount;
            consultant.RateMeanStars = newMean;
            await _context.SaveChangesAsync();

            return newItem;
        }

        public Task<List<Rating>> FindAllByConsultantIdAsync(long consultantId, params string[] includes)
        {
            return this._context.Ratings
                .IncludeMultiple(includes)
                .Where(x => x.Appointment.Service.UserId == consultantId)
                .ToListAsync();
        }
    }
}
