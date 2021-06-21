using Database.Extension;
using Database.Model;
using Database.Model.Context;
using Database.Repository.Generic;
using Database.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Repository
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task<AppointmentStep> SubmitStep(Appointment appointment, long StepId, JsonDocument SubmitData)
        {
            if (appointment == null)
                throw new Exception("empty appointment");

            var step = appointment.AppointmentSteps.FirstOrDefault(x => x.StepId == StepId);
            if (step == null)
                throw new Exception("empty step");

            step.SubmitData = JsonSerializer.Serialize(SubmitData);
            step.IsCompleted = true;
            step.DateCompleted = DateTime.UtcNow;

            if (appointment.AppointmentSteps.All(x => x.IsCompleted))
            {
                appointment.IsCompleted = true;
                appointment.EndDate = DateTime.UtcNow;
            }

            await this._context.SaveChangesAsync();

            return step;
        }

        public async Task<AppointmentStep> EditStep(Appointment appointment, long StepId, JsonDocument SubmitData)
        {
            if (appointment == null)
                throw new Exception("empty appointment");

            var step = appointment.AppointmentSteps.FirstOrDefault(x => x.StepId == StepId);
            if (step == null)
                throw new Exception("empty step");

            step.SubmitData = JsonSerializer.Serialize(SubmitData);
            step.DateCompleted = DateTime.UtcNow;          

            await this._context.SaveChangesAsync();

            return step;
        }

        public Task<List<Appointment>> FindAllAsync(User requester, params string[] includes)
        {
            if (requester.IsAdmin())
                return base.FindAllAsync(false, includes);

            if (requester.IsConsultant())
            {
                return this._context.Appointments
                    .AsNoTracking()
                    .IncludeMultiple(includes)
                    .Where(p => p.Service.UserId == requester.Id)
                    .OrderByDescending(x => x.StartDate)
                    .OrderBy(x => x.IsCompleted)
                    .ToListAsync();
            }
            else
            {
                return this._context.Appointments
                    .AsNoTracking()
                    .IncludeMultiple(includes)
                    .Where(p => p.ClientId == requester.Id)
                    .OrderByDescending(x => x.StartDate)
                    .OrderBy(x => x.IsCompleted)
                    .ToListAsync();
            }
        }

        public async Task<PagedSearch<Appointment>> FindWithPagedSearchAsync(
            string clientName, User requester, 
            string sortDirection, 
            int pageSize, 
            int page,
            CancellationToken cancellationToken,
            params string[] includes)
        {
            var query = this._context.Appointments
                .AsNoTracking()
                .IncludeMultiple(includes);

            if (!requester.IsAdmin())
            {
                if (requester.IsConsultant())
                {
                    query = query.Where(p => p.Service.UserId == requester.Id);
                }
                else
                {
                    query = query.Where(p => p.ClientId == requester.Id);
                }
            }
            
            if (!String.IsNullOrWhiteSpace(clientName))
            {
                query = query.Where(x => x.Client.Name.Contains(clientName));
            }

            query = query.OrderByDescending(x => x.StartDate)
                .OrderBy(x => x.IsCompleted);

            var item = await query.PaginateAsync(page, pageSize, cancellationToken);
            return item;
        }
    }
}
