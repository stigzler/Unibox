using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Admin.Data.LiteDb;
using Unibox.Admin.Data.Models;

namespace Unibox.Admin.Services
{
    public class DatabaseService
    {
        public Database Database { get; set; }

        public DatabaseService()
        {
            // LITEDB ============================================================================
            ConnectionParameters connectionParameters = new ConnectionParameters
            {
                ConnectionType = ConnectionType.Shared,
                Filename = "Unibox.Admin.ldb",
                ReadOnly = false
            };

            Database = new Database(connectionParameters);
            Database.OpenDatabase();

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