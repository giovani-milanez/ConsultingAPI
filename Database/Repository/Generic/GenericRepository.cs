using Database.Model.Base;
using Database.Model.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Repository.Generic
{
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected DatabaseContext _context;

        private DbSet<T> dataset;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            dataset = _context.Set<T>();
        }           

        public Task<List<T>> FindAllAsync(params string[] includes)
        {
            return dataset.IncludeMultiple(includes).ToListAsync();
        }

        public Task<T> FindByIdAsync(long id, params string[] includes)
        {
            return dataset.IncludeMultiple(includes).SingleOrDefaultAsync(p => p.Id.Equals(id));
        }

        public async Task<T> CreateAsync(T item)
        {
            try
            {
                dataset.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<T> UpdateAsync(T item)
        {
            var result = await dataset.SingleOrDefaultAsync(p => p.Id.Equals(item.Id));
            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(item);
                    await _context.SaveChangesAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            else
            {
                return null;
            }
        }
        public async Task DeleteAsync(long id)
        {
            var result = await dataset.SingleOrDefaultAsync(p => p.Id.Equals(id));
            if (result != null)
            {
                try
                {
                    dataset.Remove(result);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }    
        public async Task<bool> ExistsAsync(long id)
        {
            return await dataset.AnyAsync(p => p.Id.Equals(id));
        }

        public Task<List<T>> FindWithPagedSearchAsync(string query, params string[] includes)
        {
            return dataset.FromSqlRaw<T>(query).IncludeMultiple(includes).ToListAsync();
        }

        public async Task<int> GetCountAsync(string query)
        {
            var result = "";
            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    var obj = await command.ExecuteScalarAsync();
                    result = obj.ToString();
                }
            }
            return int.Parse(result);
        }
    }

    internal static class DataAccessExtensions
    {
        internal static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query,
            params string[] includes) where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query;
        }
    }
}
