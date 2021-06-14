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
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        public ServiceRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task<Service> UpdateManuallyAsync(Service item)
        {
            var result = await this._context.Services
                .IncludeMultiple(nameof(Service.ServicesSteps))
                .SingleOrDefaultAsync(p => p.Id.Equals(item.Id));
            if (result != null)
            {
                foreach (var step in item.ServicesSteps)
                {
                    step.ServiceId = item.Id;
                }
                result.Title = item.Title;
                result.Description = item.Description;
                result.ServicesSteps.Clear();
                result.ServicesSteps = item.ServicesSteps;
                await _context.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public Task<List<Service>> FindAllAsync(User requester, params string[] includes)
        {
            if (requester.IsAdmin())
                return base.FindAllAsync(includes);

            return this._context.Services
                .IncludeMultiple(includes)
                .Where(p => p.UserId == requester.Id)
                .ToListAsync();
        }

        public async Task<PagedSearch<Service>> FindWithPagedSearchAsync(string title, User requester, string sortDirection, int pageSize, int page)
        {
            var sort = (!string.IsNullOrWhiteSpace(sortDirection) && !sortDirection.Equals("desc")) ? "asc" : "desc";
            var size = (pageSize < 1) ? 10 : pageSize;
            var offset = page > 0 ? (page - 1) * size : 0;

            string query = @"select * from services p where 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(title))
            {
                query += $" and p.title like '%{title}%' ";
            }
            if (!requester.IsAdmin())
                query += $" and p.user_id = {requester.Id} ";

            query += $" order by p.title {sort} limit {size} offset {offset}";

            string countQuery = "select count(*) from services p where 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(title))
            {
                countQuery += $" and p.title like '%{title}%' ";
            }
            if (!requester.IsAdmin())
                countQuery += $" and p.user_id = {requester.Id} ";

            var items = await base.FindWithPagedSearchAsync(query);
            int totalResults = await base.GetCountAsync(countQuery);
            return new PagedSearch<Service>
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
