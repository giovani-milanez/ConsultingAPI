using Database;
using Database.Model.Context;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();

            try
            {
                var connectionString = Configuration["DbConnection:ConnectionString"];

                IDBConnection dbCon = new MySqlDBConnection();
                var evolve = new Evolve.Evolve(dbCon.GetConnection(connectionString), msg => Log.Information(msg))
                {
                    Locations = new List<string> { "db/migrations", "db/dataset" },
                    IsEraseDisabled = true
                };
                evolve.Migrate();
            }
            catch (Exception ex)
            {
                Log.Error("Database migraation failed", ex);
            }
        }
    }
}
