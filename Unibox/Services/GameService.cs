using CommunityToolkit.Mvvm.Messaging;
using stigzler.ScreenscraperWrapper.Data;
using stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper;
using stigzler.ScreenscraperWrapper.Data.Entities.Supplemental;
using stigzler.ScreenscraperWrapper.DTOs;
using stigzler.ScreenscraperWrapper.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unibox.Data.Constants;
using Unibox.Data.Models;
using Unibox.Data.ServiceOperationOutcomes;
using Unibox.Messages;

namespace Unibox.Services
{
    internal class GameService
    {
        private DatabaseService databaseService;
        private ScreenscraperService screenscraperService;
        private FileService fileService;

        public GameService(DatabaseService databaseService, ScreenscraperService screenscraperService, FileService fileService)
        {
            this.databaseService = databaseService;
            this.screenscraperService = screenscraperService;
            this.fileService = fileService;
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
                    Notes = platformElement.Element("Notes")?.Value
                };
                lbGames.Add(lbGame);
            }
            return lbGames;
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
        /// Asumes target path has already been checked for duplicate file
        /// </summary>
        /// <param name="xmlFilepath"></param>
        /// <param name="romFolder"></param>
        /// <param name="romFilePath"></param>
        /// <returns></returns>
        internal async Task<AddGameOutcome> AddRoms(string xmlFilepath, string romFolder, string romFilePath, PlatformModel platformModel, InstallationModel installationModel)
        {
            // Please note - I don't knwo what happened to me here. I started coding like it was 10 years ago
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

                            List<stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper.Game> ssGames =
                                ssResult.DataObject as List<stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper.Game>;

                            stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper.Game ssGame = ssGames.FirstOrDefault();

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

                            outcome.Outcomes.Add("Attempting to download any media for game...");

                            HashSet<string> ssSystemMediaThatMapped = new HashSet<string>();
                            // yeah - running out of steam at this point...
                            foreach (var mappedSsMediaType in databaseService.Database.Collections.LbSsMediaTypeMap.FindAll())
                            {
                                ssSystemMediaThatMapped.Add(mappedSsMediaType.SsMediaType.Name);
                            }

                            List<ApiFileDownloadParameters> mediaList = new List<ApiFileDownloadParameters>();
                            int count = 1;

                            var dave = databaseService.Database.Collections.LbMediaTypes.FindAll()
                                .Where(mt => mt.Name == "Box Front").FirstOrDefault();

                            foreach (GameMediaDetails gameMediaDetails in ssGame.MediaList)
                            {
                                if (ssSystemMediaThatMapped.Contains(gameMediaDetails.MediaType.ToString()))
                                {
                                    outcome.Outcomes.Add($"Adding media to download list: {count}. {gameMediaDetails.MediaType}");

                                    SsMediaType ssMediaType = databaseService.Database.Collections.SsMediaTypes
                                        .Find(s => s.Name == gameMediaDetails.MediaType.ToString()).FirstOrDefault();

                                    LbSsMediaTypeMap mediaMapRow = (LbSsMediaTypeMap)databaseService.Database.Collections.LbSsMediaTypeMap.FindAll()
                                        .Where(mmr => mmr.SsMediaType.Name == ssMediaType.Name).FirstOrDefault();

                                    var lbMediaType = mediaMapRow.LbMediaType;

                                    var lbPlatformFolder = platformModel.PlatformFolders.Where(pf => pf.MediaType.Name == lbMediaType.Name).FirstOrDefault();

                                    mediaList.Add(new ApiFileDownloadParameters
                                    {
                                        AssociatedUserDataObject = ssGame,
                                        Filename = Path.Combine(lbPlatformFolder.ResolvedMediaPath,
                                        $"{Path.GetFileNameWithoutExtension(romFilePath)}[0].{gameMediaDetails.Format}"),
                                        Url = gameMediaDetails.Uri
                                    });
                                    count += 1;
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
                    SecondaryMessage = "Copying Rom file and updating metadata..."
                }));

                //File.Copy(romFilePath, Path.Combine(romFolder, Path.GetFileName(romFilePath)));

                await fileService.CopyFileAsync(
                    romFilePath,
                    Path.Combine(romFolder, Path.GetFileName(romFilePath)),
                    percentMsg => WeakReferenceMessenger.Default.Send(
                        new ProgressMessage(new ProgressMessageArgs { SecondaryMessage = $"Copying file. {percentMsg} Completed." })
                        ),
                    CancellationToken.None // supply a CancellationToken if you have one, or use CancellationToken.None
                        );

                outcome.Outcomes.Add($"Copied rom to: {Path.Combine(romFolder, Path.GetFileName(romFilePath))}");

                xmlDoc.Save(xmlFilepath);
                outcome.Outcomes.Add("Added Game to Launchbox database");

                outcome.RomAdded = true;
            }

            return outcome;
        }
    }
}