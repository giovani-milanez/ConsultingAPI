using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Database.Model.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {

        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Step> Steps { get; set; }
    }
}
