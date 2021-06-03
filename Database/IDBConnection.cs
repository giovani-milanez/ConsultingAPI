using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public interface IDBConnection
    {
        DbConnection GetConnection(string conString);
        void UseDb(DbContextOptionsBuilder builder, string conString);
    }
}
