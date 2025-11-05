using CommunityToolkit.Mvvm.Messaging;
using stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper;
using stigzler.ScreenscraperWrapper.Data.Entities.Supplemental;
using stigzler.ScreenscraperWrapper.DTOs;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml.Linq;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Data.Constants;
using Unibox.Data.Models;
using Unibox.Data.ServiceOperationOutcomes;
using Unibox.Helpers;
using Unibox.Messages;
using Unibox.Messaging.DTOs;
using Unibox.Messaging.Responses;

namespace Unibox.Services
{
    internal class GameService
    {
        private DatabaseService databaseService;
        private FileService fileService;
        private LoggingService loggingService;
        private MessagingService messagingService;
        private ScreenscraperService screenscraperService;

        public GameService(DatabaseService databaseService, ScreenscraperService screenscraperService, FileService fileService,
            MessagingService messagingService, LoggingService loggingService)
        {
            this.databaseService = databaseService;
            this.screenscraperService = screenscraperService;
            this.fileService = fileService;
            this.messagingService = messagingService;
            this.loggingService = loggingService;
        }

        /// <summary>
        /// Asumes target path has already been checked for duplicate file
        /// </summary>
        /// <param name="xmlFilepath"></param>
        /// <param name="romFolder"></param>
        /// <param name="romFilePath"></param>
        /// <returns></returns>
        internal async Task<AddGameOutcome> AddRoms(string xmlFilepath, string romFolder, string romFilePath, PlatformModel platformModel, InstallationModel installationModel)
        {
            // Please note - I don't know what happened to me here. I started coding like it was 10 years ago
            // but life's too short, I aint got much time left so if it is so, the it's
            // NESTED IFS FOR THE WIN BABY! YEAH!
            AddGameOutcome outcome = new AddGameOutcome();

            outcome.Outcomes.Add($"Starting Add Game Rom routine with: \r\n" +
                $"Rom Filepath: {romFilePath} \r\n" +
                $"Rom Folder: {romFolder} \r\n" +
                $"Launchbox XML Filepath: {xmlFilepath} \r\n" +
                $"Platform Model: {platformModel.Name} \r\n" +
                $"Installation Model: {installationModel.InstallationPath}");

            System.Xml.Linq.XDocument xmlDoc = System.Xml.Linq.XDocument.Load(xmlFilepath);

            GameModel newGameModel = new GameModel
            {
                Title = Helpers.String.RemoveTags(Path.GetFileNameWithoutExtension(romFilePath)),
                ApplicationPath = Path.Combine(romFolder, Path.GetFileName(romFilePath)),
            };

            outcome.Game = newGameModel;

            outcome.Outcomes.Add($"Game name derived from Rom: {newGameModel.Title}");

            bool noMatchFoundInScreenscraper = false;

            if (Properties.Settings.Default.UseSsForRomAdds)
            {
                outcome.Outcomes.Add("Use Screenscraper selected. Attempting scrape...");
                if (platformModel.LaunchboxScrapeAs is null)
                {
                    outcome.Outcomes.Add("Cannot scrape as the Platform 'ScrapeAs' is not set in Launchbox. Please set this in launchbox to the Platform you want to scrapes as and then update the Platform in Unibox");
                    noMatchFoundInScreenscraper = true;
                }
                else
                {
                    // USE SCREENSCRAPER
                    SsSystem? ssSystem = databaseService.Database.Collections.LbSsSystemsMap.FindOne(
                        sm => sm.LbPlatform.Name == platformModel.LaunchboxScrapeAs).SsSystem;

                    if (ssSystem == null)
                    {
                        outcome.Outcomes.Add("Could not find a launchbox>screenscraper system map for this platform. Cannot proceed with scrape.");
                        noMatchFoundInScreenscraper = true;
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(new ProgressMessage(new ProgressMessageArgs
                        {
                            SecondaryMessage =
                            "Getting Game data from Screenscraper..."
                        }));

                        var ssResult = await screenscraperService.GetGameByRomName(Path.GetFileName(romFilePath), ssSystem.SsID);

                        if (ssResult.OverallOutcome != stigzler.ScreenscraperWrapper.Data.Enums.OverallOutcome.FullySuccessful)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.AppendLine(ssResult.OverallOutcome.ToString());
                            stringBuilder.AppendLine($" Http code: {ssResult.HttpStatusCode}. ");
                            if (ssResult.TextResult != null)
                            {
                                stringBuilder.AppendLine($"Screenscraper return: {ssResult.TextResult}. ");
                            }
                            if (ssResult.Exception != null)
                            {
                                stringBuilder.AppendLine($"Any exceptions: {ssResult.Exception.Message}");
                            }

                            outcome.Outcomes.Add($"Scrape did not complete successfully. Outcome: \r\n\r\n {stringBuilder.ToString()}");

                            noMatchFoundInScreenscraper = true;
                        }
                        else
                        {
                            // METADATA ---------------------------------------------------------------------------------------------------
                            WeakReferenceMessenger.Default.Send(new ProgressMessage(new ProgressMessageArgs
                            {
                                SecondaryMessage = "Game data retrieved. Populating Metadata.."
                            }));

                            List<Game> ssGames = ssResult.DataObject as List<Game>;

                            Game ssGame = ssGames.FirstOrDefault();

                            newGameModel.Title = ssGame.Names.First().Value;
                            newGameModel.Publisher = ssGame.Publisher.Value;
                            newGameModel.Developer = ssGame.Developer.Value;
                            if (ssGame.ReleaseDates.Count() > 0) newGameModel.ReleaseDate = ssGame.ReleaseDates.First().Value;
                            newGameModel.Notes = ssGame.Synopses.First().Value;

                            outcome.Outcomes.Add($"Metadata scraped successfully for game. Game name now: {newGameModel.Title}");

                            // MEDIA ---------------------------------------------------------------------------------------------------

                            WeakReferenceMessenger.Default.Send(new ProgressMessage(new ProgressMessageArgs
                            {
                                SecondaryMessage = "Discerning valid game Media"
                            }));

                            outcome.Outcomes.Add("Calculating Media downloads:");

                            HashSet<string> ssSystemMediaThatMapped = new HashSet<string>();
                            // yeah - running out of steam at this point...
                            foreach (var mappedSsMediaType in databaseService.Database.Collections.LbSsMediaTypeMap.FindAll())
                            {
                                ssSystemMediaThatMapped.Add(mappedSsMediaType.SsMediaType.Name);
                            }

                            List<ApiFileDownloadParameters> mediaList = new List<ApiFileDownloadParameters>();
                            int count = 1;

                            foreach (string ssMediaType in ssSystemMediaThatMapped)
                            {
                                GameMediaDetails ssGameMediaDetails = null;
                                var ssMedias = ssGame.MediaList.Where(m => m.MediaType.ToString() == ssMediaType);
                                if (ssMedias.Count() == 0)
                                {
                                    // do not add
                                    //outcome.Outcomes.Add($"Found media type: {ssMediaType}.");
                                }
                                else if (ssMedias.Count() > 1)
                                {
                                    foreach (string region in Properties.Settings.Default.SsRegionPriorities)
                                    {
                                        string shortRegionName = region.Split('|')[1];
                                        ssGameMediaDetails = ssMedias.Where(m => m.Region == region.Split('|')[1]).FirstOrDefault();

                                        if (ssGameMediaDetails != null)
                                        {
                                            outcome.Outcomes.Add($"{count}. {ssMediaType} Found. " +
                                                $"Prioritisation routine chose to use [{region.Split('|')[0]}]");
                                            count += 1;
                                            break;
                                        }
                                    }
                                }
                                else // count = 1
                                {
                                    ssGameMediaDetails = ssMedias.First();
                                    outcome.Outcomes.Add($"{count}. {ssMediaType} Found. " +
                                    $"Screenscraper region: [{ssGameMediaDetails.Region}]");
                                    count += 1;
                                }

                                if (ssGameMediaDetails != null)
                                {
                                    var mediaTypeMap = databaseService.Database.Collections.LbSsMediaTypeMap.
                                        FindOne(mtm => mtm.SsMediaType.Name == ssMediaType);

                                    var lbPlatformFolder = platformModel.PlatformFolders.
                                        Where(pf => pf.MediaType.Name == mediaTypeMap.LbMediaType.Name).FirstOrDefault();

                                    if (lbPlatformFolder != null)
                                    {
                                        mediaList.Add(new ApiFileDownloadParameters()
                                        {
                                            AssociatedUserDataObject = ssGame,
                                            Url = ssGameMediaDetails.Uri,
                                            Filename = Path.Combine(lbPlatformFolder.ResolvedMediaPath,
                                                $"{Path.GetFileNameWithoutExtension(romFilePath)}-00.{ssGameMediaDetails.Format}")
                                        });
                                    }
                                }
                            }

                            WeakReferenceMessenger.Default.Send(new ProgressMessage(new ProgressMessageArgs
                            {
                                SecondaryMessage = $"Downloading {mediaList.Count()} media items. This can take a bit of time..."
                            }));

                            var mediaResults = await screenscraperService.GetMediaFiles(mediaList);

                            outcome.Outcomes.Add("Attempted media downloads. Results:");

                            count = 1;
                            foreach (var mediaResult in mediaResults)
                            {
                                StringBuilder outcomeString = new StringBuilder();
                                //outcomeString.Append($"{((Game)mediaResult.AssociatedFileDownloadParameters.AssociatedUserDataObject).}: ");
                                outcomeString.Append($"{count}. Outcome: {mediaResult.OverallOutcome.ToString()}. ");
                                if (mediaResult.OverallOutcome != stigzler.ScreenscraperWrapper.Data.Enums.OverallOutcome.FullySuccessful)
                                {
                                    outcomeString.Append($"Http code: {mediaResult.HttpStatusCode}. ");
                                    if (mediaResult.TextResult != null)
                                    {
                                        outcomeString.Append($"Screenscraper return: [{mediaResult.TextResult}]. ");
                                    }
                                    if (mediaResult.Exception != null)
                                    {
                                        outcomeString.Append($"Any exceptions: {mediaResult.Exception.Message}");
                                        if (mediaResult.Exception.InnerException != null)
                                            outcomeString.Append($"Details: {mediaResult.Exception.InnerException}");
                                    }
                                }
                                outcome.Outcomes.Add($"{outcomeString.ToString()}");
                                count += 1;
                            }
                        }
                    }
                }
            }

            GameDTO gameDTO = new GameDTO
            {
                Title = newGameModel.Title,
                ApplicationPath = newGameModel.ApplicationPath,
                Developer = newGameModel.Developer,
                Publisher = newGameModel.Publisher,
                ReleaseDate = newGameModel.ReleaseDate,
                Notes = newGameModel.Notes,
                Platform = platformModel.Name,
                DateAdded = DateTime.Now,
                EmulatorID = GetEmulatorIdForPlatform(platformModel.Name, installationModel.InstallationPath)
            };

            if (Properties.Settings.Default.UseSsForRomAdds &&
                Properties.Settings.Default.StopRomAddOnNoScreenscraperMatch &&
                noMatchFoundInScreenscraper)
            {
                outcome.Outcomes.Add($"No screenscraper match found for this rom/Platform combination and Settings dictate " +
                    $"not to add Roms in this situation. Not adding rom. Rom: \r\n \r\n {romFilePath}");
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new ProgressMessage(new ProgressMessageArgs
                {
                    SecondaryMessage = "Attempting to update metadata and copy file."
                }));

                bool metadataUpdateSuccessful = false;

                AddGameResponse addGameResponse = await messagingService.SendAddGameRequest(installationModel.InstallationPath, gameDTO);

                if (addGameResponse.IsSuccessful)
                {
                    outcome.Outcomes.Add($"Game added to Launchbox database successfully. Game: {gameDTO.Title}");
                    loggingService.WriteLine($"Game added to Launchbox database successfully. Game: {gameDTO.Title}");
                    metadataUpdateSuccessful = true;
                }
                else
                {
                    outcome.Outcomes.Add($"Failed to add game to Launchbox database via the plugin. Error: {addGameResponse.TextResult}");
                    loggingService.WriteLine($"Game NOT added via plugin to Launchbox database. Error: {addGameResponse.TextResult}");
                    outcome.RomAdded = false;

                    if (Properties.Settings.Default.AllowOfflineGameUpdate)
                    {
                        outcome.Outcomes.Add($"Settings dictate to allow offline update of launchbox database. Attempting to update the xml directly.");
                        loggingService.WriteLine($"Settings dictate to allow offline update of launchbox database. Attempting to update the xml directly.");

                        if (Properties.Settings.Default.BackupPlatformXml)
                        {
                            string backupFilePath = Path.Combine(installationModel.InstallationPath, Paths.LaunchboxRelDataDir, "UniboxBackups",
                                Path.GetFileName(xmlFilepath));
                            if (!Directory.Exists(Path.GetDirectoryName(backupFilePath))) Directory.CreateDirectory(Path.GetDirectoryName(backupFilePath));
                            await fileService.CopyFileAsync(xmlFilepath, backupFilePath, percentMsg => WeakReferenceMessenger.Default.Send(
                                new ProgressMessage(new ProgressMessageArgs { SecondaryMessage = $"Backing up xml file. {percentMsg} Completed." })
                                ), CancellationToken.None);
                            outcome.Outcomes.Add($"Backed up xml file to: {backupFilePath}");
                        }

                        XElement newGameElement = new XElement("Game",
                        new XElement("Title", newGameModel.Title),
                        new XElement("ApplicationPath", newGameModel.ApplicationPath),
                        new XElement("Developer", newGameModel.Developer),
                        new XElement("Publisher", newGameModel.Publisher),
                        new XElement("ReleaseDate", ""),
                        new XElement("Notes", newGameModel.Notes),
                        new XElement("Platform", platformModel.Name),
                        new XElement("DateAdded", DateTime.Now.ToString("o")),
                        new XElement("Emulator", GetEmulatorIdForPlatform(platformModel.Name, installationModel.InstallationPath)));

                        if (newGameModel.ReleaseDate.ToString() != "01/01/0001 00:00:00") newGameElement.Element("ReleaseDate").Value =
                                newGameModel.ReleaseDate.ToString(@"yyyy-MM-dd");

                        xmlDoc.Root.Add(newGameElement);

                        xmlDoc.Save(xmlFilepath);

                        outcome.Outcomes.Add($"PLatform xml updated successfully: {xmlFilepath}");
                        loggingService.WriteLine($"PLatform xml updated successfully: {xmlFilepath}");
                        metadataUpdateSuccessful = true;
                    }
                    else
                    {
                        outcome.Outcomes.Add($"Allow offline update in settings not enabled. Cannot add game metadata. Aborting game addition.");
                        loggingService.WriteLine($"Allow offline update in settings not enabled. Cannot add game metadata. Aborting game addition.");
                    }
                }

                if (metadataUpdateSuccessful)
                {
                    // db update successful, therefore do file copy
                    await fileService.CopyFileAsync(
                        romFilePath,
                        Path.Combine(romFolder, Path.GetFileName(romFilePath)),
                        percentMsg => WeakReferenceMessenger.Default.Send(
                            new ProgressMessage(new ProgressMessageArgs { SecondaryMessage = $"Copying file. {percentMsg} Completed." })
                            ), CancellationToken.None);

                    outcome.Outcomes.Add($"Copied rom to: {Path.Combine(romFolder, Path.GetFileName(romFilePath))}");
                    outcome.RomAdded = true;
                }
            }

            return outcome;
        }

        internal async Task<DeleteGameOutcome> DeleteGame(GameModel game, InstallationModel installationModel)
        {
            DeleteGameOutcome outcome = new DeleteGameOutcome();

            GameDTO gameDTO = new GameDTO
            {
                Title = game.Title,
                //ApplicationPath = game.ApplicationPath,
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                Notes = game.Notes,
                LaunchboxID = game.LaunchboxID
            };

            DeleteGameResponse deleteGameResponse = await messagingService.SendDeleteGameRequest(installationModel.InstallationPath, gameDTO);
            outcome.GameDeleted = deleteGameResponse.IsSuccessful;
            if (deleteGameResponse.IsSuccessful)
            {
                try
                {
                    // if (!File.Exists(game.ApplicationPath)) throw new Exception("File does not exist:");

                    File.Delete(game.ApplicationPath);
                    outcome.Outcome = $"Game metadata and file deleted successfully.";
                }
                catch (Exception e)
                {
                    outcome.GameDeleted = false;
                    outcome.Outcome = $"Deleted LB data but not Game file: {e.Message}";
                }
                loggingService.WriteLine(outcome.Outcome);
            }
            else
            {
                outcome.Outcome = $"Error: {deleteGameResponse.TextResult}";
                loggingService.WriteLine(outcome.Outcome);
            }
            return outcome;
        }

        internal async Task<EditGameOutcome> EditGame(GameModel game, InstallationModel installationModel)
        {
            EditGameOutcome outcome = new EditGameOutcome();

            GameDTO gameDTO = new GameDTO
            {
                Title = game.Title,
                //ApplicationPath = game.ApplicationPath,
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                Notes = game.Notes,
                LaunchboxID = game.LaunchboxID
            };

            EditGameResponse editGameResponse = await messagingService.SendEditGameRequest(installationModel.InstallationPath, gameDTO);

            outcome.GameEdited = editGameResponse.IsSuccessful;

            if (editGameResponse.IsSuccessful)
            {
                outcome.Outcome = $"Game edited in Launchbox database successfully. Game: {gameDTO.Title}";
                loggingService.WriteLine(outcome.Outcome);
            }
            else
            {
                outcome.Outcome = $"Messaging Error: {editGameResponse.TextResult}";
                loggingService.WriteLine(outcome.Outcome);
            }
            return outcome;
        }

        internal string GetEmulatorIdForPlatform(string platformName, string launchboxRootPath)
        {
            string emulatorXmlFilepath = Path.Combine(launchboxRootPath, Paths.LaunchboxRelDataDir, Paths.LaunchboxEmulatorsXmlFile);
            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(emulatorXmlFilepath);
            var platformElement = doc.Root.Elements("EmulatorPlatform").FirstOrDefault(p => p.Element("Platform")?.Value == platformName);
            if (platformElement != null)
            {
                return platformElement.Element("Emulator")?.Value;
            }
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="game"></param>
        /// <returns>Null if none found</returns>
        internal List<string> GetGameMediaPaths(GameModel game, string mediaName)
        {
            List<string> mediaSources = new List<string>();

            PlatformFolderModel platformFolder = game.Platform.PlatformFolders.
                Where(pf => pf.MediaType.Name == mediaName).FirstOrDefault();

            if (platformFolder == null) return null;

            string romPath = game.ApplicationPath;
            string gameName = game.Title;

            // Get matches on  romName (e.g. Aliens (USA)-00.png)
            string romRegexPattern = $"^{Regex.Escape(Path.GetFileNameWithoutExtension(romPath))}(-\\d{{2}})?\\.*$";

            string nameRegexPattern = $"^{Regex.Escape(gameName)}(-\\d{{2}})?\\.*$";

            var matches = Directory.GetFiles(platformFolder.ResolvedMediaPath, "*.*", SearchOption.AllDirectories)
                .Where(f => Regex.IsMatch(Path.GetFileNameWithoutExtension(f), romRegexPattern, RegexOptions.IgnoreCase) ||
                            Regex.IsMatch(Path.GetFileNameWithoutExtension(f), nameRegexPattern, RegexOptions.IgnoreCase));

            if (matches != null)
                mediaSources = matches.ToList<string>();

            return mediaSources;
        }

        internal ObservableCollection<GameModel> GetGamesFromXml(string xmlFilepath)
        {
            ObservableCollection<GameModel> lbGames = new ObservableCollection<GameModel>();

            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(xmlFilepath);

            foreach (var platformElement in doc.Root.Elements("Game"))
            {
                GameModel lbGame = new GameModel
                {
                    Title = platformElement.Element("Title")?.Value,
                    ApplicationPath = platformElement.Element("ApplicationPath")?.Value,
                    Developer = platformElement.Element("Developer")?.Value,
                    Publisher = platformElement.Element("Publisher")?.Value,
                    ReleaseDate = DateTime.TryParse(platformElement.Element("ReleaseDate")?.Value, out DateTime releaseDate) ? releaseDate : DateTime.MinValue,
                    Notes = platformElement.Element("Notes")?.Value,
                    LaunchboxID = platformElement.Element("ID")?.Value
                };
                lbGames.Add(lbGame);
            }
            return lbGames;
        }
    }
}