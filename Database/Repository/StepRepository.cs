using Database.Model;
using Database.Model.Context;
using Database.Repository.Generic;
using Database.Utils;
using Microsoft.EntityFrameworkCore;
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

        public async Task<PagedSearch<Step>> FindWithPagedSearchAsync(string type, string sortDirection, int pageSize, int page)
        {
            var sort = (!string.IsNullOrWhiteSpace(sortDirection) && !sortDirection.Equals("desc")) ? "asc" : "desc";
            var size = (pageSize < 1) ? 10 : pageSize;
            var offset = page > 0 ? (page - 1) * size : 0;

            string query = @"select * from steps p where 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query + $" and p.type like '%{type}%' ";
            }
            query = query + $" order by p.type {sort} limit {size} offset {offset}";

            string countQuery = "select count(*) from steps p where 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(type))
            {
                countQuery += $" and p.type like '%{type}%' ";
            }
            var items = await base.FindWithPagedSearchAsync(query, "");
            int totalResults = await base.GetCountAsync(countQuery);
            return new PagedSearch<Step>
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
