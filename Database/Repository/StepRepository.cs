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
    public class StepRepository : GenericRepository<Step>, IStepRepository
    {
        public StepRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(string type)
        {
            return await _context.Steps.AnyAsync(p => p.Type.ToUpper().Equals(type.ToUpper()));
        }

        public Task<List<Step>> FindAllAsync(User requester, params string[] includes)
        {
            if (requester.IsAdmin())
                return base.FindAllAsync(false, includes);

            return this._context.Steps
                .IncludeMultiple(includes)
                .Where(p => p.UserId == null || p.UserId.Value == requester.Id)
                .ToListAsync();
        }

        public async Task<PagedSearch<Step>> FindWithPagedSearchAsync(
            User requester,
            PagedRequest paging,
            CancellationToken cancellationToken,
            params string[] includes)
        {
            var query = this._context.Steps
                .AsNoTracking()
                .IncludeMultiple(includes);

            if (!requester.IsAdmin())
            {
                query = query.Where(p => !p.UserId.HasValue || p.UserId == requester.Id);
            }

            var nameFilter = paging.Filters.FirstOrDefault(x => x.FieldName == "display_name");
            if (nameFilter != null)
            {
                var type = nameFilter.Value.ToLower();
                query = query.Where(x => x.Type.ToLower().Contains(type));
            }

            var sortById = paging.SortFields.FirstOrDefault(x => x.FieldName == "id");
            if (sortById != null)
            {
                bool desc = sortById.SortOrder == "desc";
                if (desc)
                    query = query.OrderByDescending(x => x.Id);
                else
                    query = query.OrderBy(x => x.Id);
            }
            var sortByName = paging.SortFields.FirstOrDefault(x => x.FieldName == "display_name");
            if (sortByName != null)
            {
                bool desc = sortByName.SortOrder == "desc";
                if (desc)
                    query = query.OrderByDescending(x => x.DisplayName);
                else
                    query = query.OrderBy(x => x.DisplayName);
            }

            var item = await query.PaginateAsync(paging.Page, paging.PageSize, cancellationToken);
            return item;           
        }
    }
}
