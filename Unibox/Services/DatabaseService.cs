using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.LiteDb;

namespace Unibox.Services
{
    public class DatabaseService
    {
        public Database Database { get; set; }

        public DatabaseService()
        {
            ConnectionParameters connectionParameters = new ConnectionParameters
            {
                ConnectionType = ConnectionType.Shared,
                Filename = "Unibox.db",
                ReadOnly = false
            };

            Database = new Database(connectionParameters);
            Database.OpenDatabase();
            //Database.Collections.Installations.Insert(new Data.Models.InstallationModel { InstallationPath = "Dave", Name = "Test Installation" });
            //Database.CloseDatabase();


            if (Database.ConnectionException != null)
            {
                throw new Exception("Failed to open database: " + Database.ConnectionException.Message);
              
            }
            
            if (!Database.DatabaseOpen)
            {
                throw new Exception("Database is not open.");
            }
        }
    }
}
