using CommunityToolkit.Mvvm.Messaging;
using LiteDB;
using System.IO;
using System.Reflection;
using Unibox.Data.LiteDb;
using Unibox.Data.Models;

namespace Unibox.Services
{
    public class DatabaseService
    {
        public Database Database { get; set; }

        private Messages.ProgressMessageArgs progressMessageArgs = new Messages.ProgressMessageArgs();

        public DatabaseService()
        {
            // LITEDB ============================================================================
            ConnectionParameters connectionParameters = new ConnectionParameters
            {
                ConnectionType = ConnectionType.Shared,
                Filename = "Unibox.ldb",
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

            EnsureTextBasedDataPopulated();
        }

        /// <summary>
        /// This ensures that data based on that held in the Data/[various].dat is
        /// populated into LiteDb models. Normally occurs at startup.
        /// </summary>
        internal void EnsureTextBasedDataPopulated()
        {
            // Screenscraper Systems
            if (Database.Collections.SsSystems.Count() == 0)
            {
                var file = File.ReadAllLinesAsync(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.SsSystemsFile));
                string[] lines = file.Result;

                List<SsSystem> ssSystemsCollection = new List<SsSystem>();

                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    var ssSystem = new SsSystem { Name = parts[1], SsID = Convert.ToInt32(parts[0]) };
                    ssSystemsCollection.Add(ssSystem);
                }
                Database.Collections.SsSystems.InsertBulk(ssSystemsCollection);
            }

            // Screenscraper Media Types
            if (Database.Collections.SsMediaTypes.Count() == 0)
            {
                var file = File.ReadAllLinesAsync(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.SsMediaTypesFile));
                string[] lines = file.Result;
                List<SsMediaType> ssMediaTypesCollection = new List<SsMediaType>();
                foreach (var line in lines)
                {
                    var ssMediaType = new SsMediaType { Name = line };
                    ssMediaTypesCollection.Add(ssMediaType);
                }
                Database.Collections.SsMediaTypes.Insert(ssMediaTypesCollection);
            }

            // Launchbox Platforms
            if (Database.Collections.LbPlatforms.Count() == 0)
            {
                var file = File.ReadAllLinesAsync(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.LbPlatformsFile));
                string[] lines = file.Result;
                List<LbPlatform> lbPlatformsCollection = new List<LbPlatform>();
                foreach (var line in lines)
                {
                    // Assuming the line is a valid XML representation of a PlatformModel
                    var lbPlatform = new LbPlatform { Name = line }; // Simplified for example
                    lbPlatformsCollection.Add(lbPlatform);
                }
                Database.Collections.LbPlatforms.Insert(lbPlatformsCollection);
            }

            // Launchbox Media Types
            if (Database.Collections.LbMediaTypes.Count() == 0)
            {
                var file = File.ReadAllLinesAsync(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.LbMediaTypesFile));
                string[] lines = file.Result;
                List<LbMediaType> lbMediaTypesCollection = new List<LbMediaType>();
                foreach (var line in lines)
                {
                    // Assuming the line is a valid XML representation of a PlatformModel
                    var lbMediaType = new LbMediaType { Name = line }; // Simplified for example
                    lbMediaTypesCollection.Add(lbMediaType);
                }
                Database.Collections.LbMediaTypes.Insert(lbMediaTypesCollection);
            }

            // Launcbox to Screenscraper Media Type Map
            if (Database.Collections.LbSsMediaTypeMap.Count() == 0)
            {
                var file = File.ReadAllLinesAsync(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.MediaMapFile));
                string[] lines = file.Result;
                List<LbSsMediaTypeMap> lbSsMediaTypeMaps = new List<LbSsMediaTypeMap>();
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        var lbSsMediaTypeMap = new LbSsMediaTypeMap
                        {
                            LbMediaType = Database.Collections.LbMediaTypes.FindAll().Where(m => m.Name == parts[0]).FirstOrDefault(),
                        };

                        lbSsMediaTypeMap.SsMediaType = Database.Collections.SsMediaTypes.FindAll().Where(m => m.Name == parts[1]).FirstOrDefault();

                        //foreach (string ssMediaType in parts[1].Split(','))
                        //{
                        //    lbSsMediaTypeMap.SsMediaType.Add(Database.Collections.SsMediaTypes.FindAll().Where(m => m.Name == ssMediaType).FirstOrDefault());
                        //}

                        lbSsMediaTypeMaps.Add(lbSsMediaTypeMap);
                    }
                }
                Database.Collections.LbSsMediaTypeMap.Insert(lbSsMediaTypeMaps);
            }

            // Launchbox to Screenscraper Systems Map
            if (Database.Collections.LbSsSystemsMap.Count() == 0)
            {
                var file = File.ReadAllLinesAsync(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.PlatformMapFile));
                string[] lines = file.Result;
                List<LbSsSystemMap> lbSsSystemMaps = new List<LbSsSystemMap>();
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        var lbPlatform = Database.Collections.LbPlatforms.FindAll().Where(p => p.Name == parts[0]).FirstOrDefault();
                        var ssSystem = Database.Collections.SsSystems.FindAll().Where(s => s.SsID == Convert.ToInt32(parts[1])).FirstOrDefault();
                        var lbSsSystemMap = new LbSsSystemMap
                        {
                            LbPlatform = lbPlatform,
                            SsSystem = ssSystem
                        };
                        lbSsSystemMaps.Add(lbSsSystemMap);
                    }
                }
                Database.Collections.LbSsSystemsMap.Insert(lbSsSystemMaps);
            }
        }
    }
}