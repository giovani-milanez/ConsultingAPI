using Database.Extension;
using Database.Model;
using Database.Model.Context;
using Database.Repository.Generic;
using Database.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                return base.FindAllAsync(false, includes);

            return this._context.Services
                .IncludeMultiple(includes)
                .AsNoTracking()
                .Where(p => p.UserId == requester.Id)
                .ToListAsync();
        }

        public async Task<PagedSearch<Service>> FindWithPagedSearchAsync(
            string title, 
            User requester, 
            string sortDirection, 
            int pageSize, 
            int page,
            CancellationToken cancellationToken,
            params string[] includes)
        {
            var query = this._context.Services
                .AsNoTracking()
                .IncludeMultiple(includes);

            if (!requester.IsAdmin())
            {
                query = query.Where(p => p.UserId == requester.Id);
            }

            if (!String.IsNullOrWhiteSpace(title))
            {
                query = query.Where(x => x.Title.Contains(title));
            }

            query = query.OrderByDescending(x => x.Id);

            var item = await query.PaginateAsync(page, pageSize, cancellationToken);
            return item;            
        }
    }
}
