using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unibox.Data.Models;
using Unibox.Data.ServiceOperationOutcomes;
using stigzler.ScreenscraperWrapper.Data;
using stigzler.ScreenscraperWrapper.Services;
using stigzler.ScreenscraperWrapper.DTOs;
using stigzler.ScreenscraperWrapper.Data.Entities.Supplemental;

namespace Unibox.Services
{
    internal class GameService
    {
        private DatabaseService databaseService;
        private ScreenscraperService screenscraperService;

        public GameService(DatabaseService databaseService, ScreenscraperService screenscraperService)
        {
            this.databaseService = databaseService;
            this.screenscraperService = screenscraperService;
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

        /// <summary>
        /// Asumes target path has already been checked for duplicate file
        /// </summary>
        /// <param name="xmlFilepath"></param>
        /// <param name="romFolder"></param>
        /// <param name="romFilePath"></param>
        /// <returns></returns>
        internal async Task<AddGameOutcome> AddRoms(string xmlFilepath, string romFolder, string romFilePath, PlatformModel platformModel)
        {
            // Please note - I don't knwo what happened to me here. I started coding like it was 10 years ago
            // but life's too short, I aint got much time left so if it is so, the it's
            // NESTED IFS FOR THE WIN BABY! YEAH!
            AddGameOutcome outcome = new AddGameOutcome();

            System.Xml.Linq.XDocument xmlDoc = System.Xml.Linq.XDocument.Load(xmlFilepath);

            //File.Copy(romFilePath, Path.Combine(romFolder, Path.GetFileName(romFilePath)));

            GameModel newGameModel = new GameModel
            {
                Title = Helpers.String.RemoveTags(Path.GetFileNameWithoutExtension(romFilePath)),
                ApplicationPath = Path.Combine(romFolder, Path.GetFileName(romFilePath)),
            };

            outcome.Game = newGameModel;

            if (Properties.Settings.Default.UseSsForRomAdds)
            {
                outcome.Outcomes.Add("Use Screenscraper selected. Attempting scrape...");
                if (platformModel.LaunchboxScrapeAs is null)
                {
                    outcome.Outcomes.Add("Cannot scrape as the Platform 'ScrapeAs' is not set in Launchbox. Please set this in launchbox to the Platform you want to scrapes as and then update the Platform in Unibox");
                }
                else
                {
                    SsSystem? ssSystem = databaseService.Database.Collections.LbSsSystemsMap.FindOne(
                        sm => sm.LbPlatform.Name == platformModel.LaunchboxScrapeAs).SsSystem;

                    if (ssSystem == null)
                    {
                        outcome.Outcomes.Add("Could not find a launchbox>screenscraper system map for this platform. Cannot proceed with scrape.");
                    }
                    else
                    {
                        var ssResult = await screenscraperService.GetGameByRomName(Path.GetFileName(romFilePath), ssSystem.SsID);
                        if (ssResult.OverallOutcome != stigzler.ScreenscraperWrapper.Data.Enums.OverallOutcome.FullySuccessful)
                        {
                            outcome.Outcomes.Add($"Scrape did not complete successfully. Outcome: {ssResult.OverallOutcome.ToString()}: " +
                                $" Http code: {ssResult.HttpStatusCode}. Screenscraper return: {ssResult.TextResult}. " +
                                $"Any exceptions: {ssResult?.Exception.Message}");
                        }
                        else
                        {
                            // METADATA

                            List<stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper.Game> ssGames =
                                ssResult.DataObject as List<stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper.Game>;

                            stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper.Game ssGame = ssGames.FirstOrDefault();

                            newGameModel.Title = ssGame.Names.First().Value;
                            newGameModel.Publisher = ssGame.Publisher.Value;
                            newGameModel.Developer = ssGame.Developer.Value;
                            if (ssGame.ReleaseDates.Count() > 0) newGameModel.ReleaseDate = ssGame.ReleaseDates.First().Value;
                            newGameModel.Notes = ssGame.Synopses.First().Value;

                            outcome.Outcomes.Add("Metadata scraped successfully for game");

                            // MEDIA
                            HashSet<string> ssSystemMediaThatMapped = new HashSet<string>();
                            // yeah - running out of steam at this point...
                            foreach (var mappedSsMediaType in databaseService.Database.Collections.LbSsMediaTypeMap.FindAll())
                            {
                                if (mappedSsMediaType.SsMediaType.Count() > 0)
                                {
                                    foreach (var ssMediaType in mappedSsMediaType.SsMediaType)
                                    {
                                        ssSystemMediaThatMapped.Add(ssMediaType.Name);
                                    }
                                }
                            }

                            List<ApiFileDownloadParameters> mediaList = new List<ApiFileDownloadParameters>();

                            foreach (GameMediaDetails gameMediaDetails in ssGame.MediaList)
                            {
                                if (ssSystemMediaThatMapped.Contains(gameMediaDetails.MediaType.ToString()))
                                {
                                    outcome.Outcomes.Add($"Adding media to download list: {gameMediaDetails.MediaType} - {gameMediaDetails.Uri}");

                                    SsMediaType ssMediaType = databaseService.Database.Collections.SsMediaTypes
                                        .Find(s => s.Name == gameMediaDetails.MediaType.ToString()).FirstOrDefault();

                                    var mediaMapRow = databaseService.Database.Collections.LbSsMediaTypeMap.FindAll()
                                        .Where(mmr => mmr.SsMediaType == ssMediaType);

                                    var lbMediaType = mediaMapRow?.LbMediaType;

                                    var lbPlatformFolder = platformModel.PlatformFolders.Where(pf => pf.MediaType == lbMediaType).FirstOrDefault();

                                    mediaList.Add(new ApiFileDownloadParameters
                                    {
                                        AssociatedUserDataObject = ssGame,
                                        Filename = Path.Combine(lbPlatformFolder.ResolvedMediaPath,
                                        $"{Path.GetFileNameWithoutExtension(romFilePath)}[0].{gameMediaDetails.Format}"),
                                        Url = gameMediaDetails.Uri
                                    });
                                }
                                //await screenscraperService.kllkj(mediaList);
                            }
                        }
                    }

                    XElement newGameElement = new XElement("Game",
                        new XElement("Title", newGameModel.Title),
                        new XElement("ApplicationPath", newGameModel.ApplicationPath),
                        new XElement("Developer", newGameModel.Developer),
                        new XElement("Publisher", newGameModel.Publisher),
                        new XElement("ReleaseDate", ""),
                        new XElement("Notes", newGameModel.Notes)
                    );

                    if (newGameModel.ReleaseDate.ToString() != "01/01/0001 00:00:00") newGameElement.Element("ReleaseDate").Value =
                            newGameModel.ReleaseDate.ToString();

                    xmlDoc.Root.Add(newGameElement);
                    xmlDoc.Save(xmlFilepath);
                }
            }
            return outcome;
        }
    }
}