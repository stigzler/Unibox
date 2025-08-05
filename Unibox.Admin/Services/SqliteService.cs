using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Admin.Services
{
    internal class SqliteService
    {
        public string Filepath { get; set; }

        public List<String> GetData(string query)
        {
            using (var connection = new SqliteConnection($"Data Source={Filepath}"))
            {
                List<string> results = new List<string>();

                connection.Open();

                var command = connection.CreateCommand();
                //command.CommandText =
                //                @"
                //        SELECT name
                //        FROM user
                //        WHERE id = $id
                //    ";
                command.CommandText = query;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);

                        results.Add(name);
                    }
                }

                return results;
            }
        }
    }
}