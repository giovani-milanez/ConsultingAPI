using Database.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Extension
{
    public static class QueryExtensios
    {
        public static async Task<PagedSearch<T>> PaginateAsync<T>(
           this IQueryable<T> query,
           int page,
           int limit,
           CancellationToken cancellationToken)
           where T : class
        {
            var paged = new PagedSearch<T>(page, limit);

            var startRow = (page - 1) * limit;
            paged.Items = await query.Skip(startRow).Take(limit).ToListAsync(cancellationToken);
            paged.TotalResults = await query.CountAsync(cancellationToken);

            return paged;
        }

        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params string[] includes) 
            where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query;
        }
    }
}
