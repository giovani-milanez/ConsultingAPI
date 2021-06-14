using Database.Extension;
using Database.Model;
using Database.Model.Context;
using Database.Repository.Generic;
using Database.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Repository
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(DatabaseContext context) : base(context)
        {
        }

        public Task<List<Appointment>> FindAllAsync(User requester, params string[] includes)
        {
            if (requester.IsAdmin())
                return base.FindAllAsync(includes);

            if (requester.IsConsultant())
            {
                return this._context.Appointments
                    .IncludeMultiple(includes)
                    .Where(p => p.Service.UserId == requester.Id)
                    .OrderByDescending(x => x.StartDate)
                    .OrderBy(x => x.IsCompleted)
                    .ToListAsync();
            }
            else
            {
                return this._context.Appointments
                    .IncludeMultiple(includes)
                    .Where(p => p.ClientId == requester.Id)
                    .OrderByDescending(x => x.StartDate)
                    .OrderBy(x => x.IsCompleted)
                    .ToListAsync();
            }
        }

        public async Task<PagedSearch<Appointment>> FindWithPagedSearchAsync(string clientName, User requester, string sortDirection, int pageSize, int page)
        {
            var sort = (!string.IsNullOrWhiteSpace(sortDirection) && !sortDirection.Equals("desc")) ? "asc" : "desc";
            var size = (pageSize < 1) ? 10 : pageSize;
            var offset = page > 0 ? (page - 1) * size : 0;

            var join = "";
            var filter = " where 1 = 1 ";
            string query = @"select * from appointments p ";
            if (!string.IsNullOrWhiteSpace(clientName))
            {
                join += " join users u on (u.id = p.client_id) ";
                filter += $" and u.name like '%{clientName}%' ";
            }

            if (requester.IsConsultant())
            {
                join += " join services s on (s.id = p.service_id) ";
                filter += $" and s.user_id = {requester.Id} ";
            }
            else if (!requester.IsAdmin())
            {
                filter += $" and p.client_id = {requester.Id} ";
            }

            query += join;
            query += filter;
            query += $" order by p.start_date desc, p.is_completed {sort} limit {size} offset {offset}";

            string countQuery = "select count(*) from appointments p ";
            var countJoin = "";
            var countFilter = " where 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(clientName))
            {
                countJoin += " join users u on (u.id = p.client_id) ";
                countFilter += $" and u.name like '%{clientName}%' ";
            }
            if (requester.IsConsultant())
            {
                countJoin += " join services s on (s.id = p.service_id) ";
                countFilter += $" and s.user_id = {requester.Id} ";
            }
            else if (!requester.IsAdmin())
            {
                countFilter += $" and p.client_id = {requester.Id} ";
            }
            countQuery += countJoin;
            countQuery += countFilter;

            var items = await base.FindWithPagedSearchAsync(query);
            int totalResults = await base.GetCountAsync(countQuery);
            return new PagedSearch<Appointment>
            {
                CurrentPage = page,
                List = items,
                PageSize = size,
                SortDirections = sort,
                TotalResults = totalResults
            };
        }
    }
}
