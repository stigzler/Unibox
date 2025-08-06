using CommunityToolkit.Mvvm.Messaging;
using LiteDB;
using System.IO;
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

            //EnsureTextBasedDataPopulated();
        }

        private void SendDatabaseInitialisationMessage(string primaryMessage = null, string secondaryMessage = null,
            int count = -1, int total = -1)
        {
            if (primaryMessage != null) progressMessageArgs.PrimaryMessage = primaryMessage;
            if (secondaryMessage != null) progressMessageArgs.SecondaryMessage = secondaryMessage;
            if (count != -1 && total != -1) progressMessageArgs.PercentageComplete = (int)((double)count / total * 100);
            WeakReferenceMessenger.Default.Send(new Messages.ProgressMessage(progressMessageArgs));
        }

        private void SendDatabaseInitialisationMessage(string primaryMessage, string secondaryMessage)
        {
            SendDatabaseInitialisationMessage(primaryMessage, secondaryMessage, -1, -1);
        }

        private void SendDatabaseInitialisationMessage(int count, int total)
        {
            SendDatabaseInitialisationMessage(null, null, count, total);
        }

        private void SendDatabaseInitialisationMessage(string secondaryMessage)
        {
            SendDatabaseInitialisationMessage(null, secondaryMessage, -1, -1);
        }

        /// <summary>
        /// This ensures that data based on that held in the Data/[various].dat is
        /// populated into LiteDb models. Normally occurs at startup.
        /// </summary>
        internal async Task EnsureTextBasedDataPopulated()
        {
            int step = 1;
            // Screenscraper Systems
            if (Database.Collections.SsSystems.Count() == 0)
            {
                SendDatabaseInitialisationMessage("First Run detected. Initialising Unibox...", $"(Step {step}/6) Updating Screenscraper Systems List...");
                var file = File.ReadAllLinesAsync(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.SsSystemsFile));
                string[] lines = file.Result;
                int count = 0;
                int total = lines.Count();

                List<SsSystem> ssSystemsCollection = new List<SsSystem>();

                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    var ssSystem = new SsSystem { Name = parts[1], SsID = Convert.ToInt32(parts[0]) };
                    SendDatabaseInitialisationMessage(count, total);
                    ssSystemsCollection.Add(ssSystem);
                    count += 1;
                }
                Database.Collections.SsSystems.InsertBulk(ssSystemsCollection);

                step += 1;
            }

            // Screenscraper Media Types
            if (Database.Collections.SsMediaTypes.Count() == 0)
            {
                SendDatabaseInitialisationMessage("First Run detected. Initialising Unibox...", $"(Step {step}/6) Updating Screenscraper Media Types List...");

                var lines = File.ReadLines(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.SsMediaTypesFile));
                int count = 0;
                int total = lines.Count();
                foreach (var line in lines)
                {
                    var ssMediaType = new SsMediaType { Name = line };
                    Database.Collections.SsMediaTypes.Insert(ssMediaType);
                    SendDatabaseInitialisationMessage(count, total);
                    count += 1;
                }
                step += 1;
            }

            // Launchbox Platforms
            if (Database.Collections.LbPlatforms.Count() == 0)
            {
                SendDatabaseInitialisationMessage("First Run detected. Initialising Unibox...", $"(Step {step}/6) Updating Launchbox Platforms List...");

                var lines = File.ReadLines(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.LbPlatformsFile));
                int count = 0;
                int total = lines.Count();
                foreach (var line in lines)
                {
                    // Assuming the line is a valid XML representation of a PlatformModel
                    var lbPlatform = new LbPlatform { Name = line }; // Simplified for example
                    Database.Collections.LbPlatforms.Insert(lbPlatform);
                    SendDatabaseInitialisationMessage(count, total);
                    count += 1;
                }
                step += 1;
            }

            // Launchbox Media Types
            if (Database.Collections.LbMediaTypes.Count() == 0)
            {
                SendDatabaseInitialisationMessage("First Run detected. Initialising Unibox...", $"(Step {step}/6) Updating Launchbox Media Types List...");

                var lines = File.ReadLines(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.LbMediaTypesFile));
                int count = 0;
                int total = lines.Count();
                foreach (var line in lines)
                {
                    // Assuming the line is a valid XML representation of a PlatformModel
                    var lbMediaType = new LbMediaType { Name = line }; // Simplified for example
                    Database.Collections.LbMediaTypes.Insert(lbMediaType);
                    SendDatabaseInitialisationMessage(count, total);
                    count += 1;
                }
                step += 1;
            }

            // Launcbox to Screenscraper Media Type Map
            if (Database.Collections.LbSsMediaTypeMap.Count() == 0)
            {
                SendDatabaseInitialisationMessage("First Run detected. Initialising Unibox...", $"(Step {step}/6) Updating Screenscraper to Launchbox Media Type Map...");

                var lines = File.ReadLines(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.MediaMapFile));
                int count = 0;
                int total = lines.Count();
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        var lbSsMediaTypeMap = new LbSsMediaTypeMap
                        {
                            LbMediaType = Database.Collections.LbMediaTypes.FindAll().Where(m => m.Name == parts[0]).FirstOrDefault(),
                        };

                        foreach (string ssMediaType in parts[1].Split(','))
                        {
                            lbSsMediaTypeMap.SsMediaType.Add(Database.Collections.SsMediaTypes.FindAll().Where(m => m.Name == ssMediaType).FirstOrDefault());
                        }

                        Database.Collections.LbSsMediaTypeMap.Insert(lbSsMediaTypeMap);
                        SendDatabaseInitialisationMessage(count, total);
                        count += 1;
                    }
                }
                step += 1;
            }

            // Launchbox to Screenscraper Systems Map
            if (Database.Collections.LbSsSystemsMap.Count() == 0)
            {
                SendDatabaseInitialisationMessage("First Run detected. Initialising Unibox...", $"(Step {step}/6) Updating Screenscraper to Launchbox Systems Map...");

                var lines = File.ReadLines(Path.Combine(AppContext.BaseDirectory,
                    Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.PlatformMapFile));
                int count = 0;
                int total = lines.Count();
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
                        Database.Collections.LbSsSystemsMap.Insert(lbSsSystemMap);
                        SendDatabaseInitialisationMessage(count, total);
                        count += 1;
                    }
                }
                step += 1;
            }
        }
    }
}