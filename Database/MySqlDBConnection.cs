using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data.Common;

namespace Database
{
    public class MySqlDBConnection : IDBConnection
    {
        public DbConnection GetConnection(string conString)
        {
            return new MySqlConnection(conString);
        }

        public void UseDb(DbContextOptionsBuilder builder, string conString)
        {
            builder.UseMySql(conString, ServerVersion.AutoDetect(conString));
        }
    }
}
