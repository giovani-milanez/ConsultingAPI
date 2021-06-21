using Database.Extension;
using Database.Model.Base;
using Database.Model.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public Task<List<T>> FindAllAsync(bool tracking, params string[] includes)
        {
            if (!tracking)
                return dataset.AsNoTracking().IncludeMultiple(includes).ToListAsync();
            
            return dataset.IncludeMultiple(includes).ToListAsync();
        }

        public Task<T> FindByIdAsync(long id, bool tracking, params string[] includes)
        {
            var query = dataset.IncludeMultiple(includes);
            
            if (!tracking)
                query = query.AsNoTracking();

            return query.SingleOrDefaultAsync(p => p.Id.Equals(id));
        }

        public async Task<T> CreateAsync(T item)
        {
            dataset.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<T> UpdateAsync(T item)
        {
            var entity = await this.FindByIdAsync(item.Id, true);
            return await this.UpdateTrackedAsync(entity, item);
        }

        public async Task<T> UpdateTrackedAsync(T trackedItem, T newValues)
        {
            if (trackedItem == null || newValues == null) return null;

            _context.Entry(trackedItem).CurrentValues.SetValues(newValues);
            await _context.SaveChangesAsync();
            return trackedItem;
        }
        public async Task DeleteAsync(long id)
        {
            var entity = await this.FindByIdAsync(id, true);
            await this.DeleteTrackedAsync(entity);
        }

        public async Task DeleteTrackedAsync(T trackedItem)
        {
            if (trackedItem != null)
            {
                dataset.Remove(trackedItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await dataset.AnyAsync(p => p.Id.Equals(id));
        }

        //public Task<List<T>> FindWithPagedSearchAsync(string query, params string[] includes)
        //{
        //    return dataset.FromSqlRaw<T>(query).AsNoTracking().IncludeMultiple(includes).ToListAsync();
        //}

        //public async Task<int> GetCountAsync(string query)
        //{
        //    var result = "";
        //    using (var connection = _context.Database.GetDbConnection())
        //    {
        //        await connection.OpenAsync();
        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = query;
        //            var obj = await command.ExecuteScalarAsync();
        //            result = obj.ToString();
        //        }
        //    }
        //    return int.Parse(result);
        //}
    }
}
