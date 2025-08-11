using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using Unibox.Data.Enums;
using Unibox.Data.LiteDb;
using Unibox.Data.MessageDetails;
using Unibox.Data.Models;
using Unibox.Data.ServiceOperationOutcomes;
using Unibox.Messages;
using Unibox.Views;

namespace Unibox.Services
{
    internal class PlatformService
    {
        private DatabaseService databaseService;
        private InstallationService installationService;

        public PlatformService(DatabaseService databaseService, InstallationService installationService)
        {
            this.databaseService = databaseService;
            this.installationService = installationService;
        }

        public UpdatePlatformsOutcome UpdateInstallationPlatforms(InstallationModel installation)
        {
            UpdatePlatformsOutcome updatePlatformsOutcome = new UpdatePlatformsOutcome();

            // CHECK FOR ERRORS

            // Check can access Installaiton Path Directory
            if (!Directory.Exists(installation.InstallationPath))
            {
                updatePlatformsOutcome.UpdatePlatformOutcome = UpdatePlatformOutcome.CannotAccessInstallationDirectory;
                return updatePlatformsOutcome;
            }

            // Populate the candidate platforms list from the PLatforms.xml file:
            ObservableCollection<PlatformModel> xmlPlatforms = GetPlatformsFromXml(installation.InstallationPath);

            // Check if the XML Platforms file was found and parsed successfully
            if (xmlPlatforms == null)
            {
                updatePlatformsOutcome.UpdatePlatformOutcome = UpdatePlatformOutcome.XmlFileDoesNotExist;
                updatePlatformsOutcome.OutcomeSummary = $"The XML Platforms xml file does not exist at: " +
                    $"{Path.Combine(installation.InstallationPath, Data.Constants.Paths.LaunchboxRelDataDir,
                Data.Constants.Paths.LaunchboxPlatformsXmlFile)}";
                return updatePlatformsOutcome;
            }

            // AT THIS POINT, PROCESS WILL COMPLETE SUCCESSFULLY, but could contain warnings

            foreach (PlatformModel launchboxPlatform in xmlPlatforms)
            {
                // Check if the platform already exists in the database
                PlatformModel installationPlatform = installation.Platforms.Where(p => p.Name == launchboxPlatform.Name).FirstOrDefault();
                if (installationPlatform == null)
                {
                    // ADD NEW PLATFORM
                    // Does not exist, add new to the database
                    updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                        UpdatePlatformMessageType.Information,
                        $"New Platform detected. Adding platform: [{launchboxPlatform.Name}] to the database.",
                        launchboxPlatform.Name));

                    installationPlatform = new PlatformModel();
                    installation.Platforms.Add(installationPlatform);
                }
                else if (installationPlatform.Locked)
                {
                    // Platform is locked - skip
                    updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                        UpdatePlatformMessageType.Warning,
                        $"Platform is locked. Skipping update: [{launchboxPlatform.Name}].",
                        installationPlatform.Name));
                    continue;
                }
                else
                {
                    updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                        UpdatePlatformMessageType.Information,
                        $"Platform already in database. Updating: [{launchboxPlatform.Name}].",
                        installationPlatform.Name));
                }

                // PLATFORMS ===============================================================================================

                installationPlatform.Name = launchboxPlatform.Name;
                installationPlatform.LaunchboxScrapeAs = launchboxPlatform.LaunchboxScrapeAs;
                installationPlatform.LaunchboxRomFolder = launchboxPlatform.LaunchboxRomFolder;

                // Check LaunchboxScrapeAs
                if (String.IsNullOrWhiteSpace(launchboxPlatform.LaunchboxScrapeAs))
                {
                    if (databaseService.Database.Collections.LbPlatforms.Exists(lbp => lbp.Name == installationPlatform.Name))
                    {
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Warning,
                            $"No ScrapeAs set in Launchbox Platform but the Name is a standard Platfrom Name. Thus, setting ScrapeAs to the Platform name.",
                            installationPlatform.Name));
                        installationPlatform.LaunchboxScrapeAs = launchboxPlatform.Name;
                    }
                    else
                    {
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Warning,
                            $"No ScrapeAs set in Launchbox. You will have to set this manually to enable metadata/media scrapes.",
                            launchboxPlatform.Name));
                    }
                }

                // ROM FOLDER OPS ------------------------------------------------------------------------------------------

                if (String.IsNullOrWhiteSpace(launchboxPlatform.LaunchboxRomFolder))
                // Rom Folder Null - try to resolve to LB Games folder  ----------------------------------------------------
                {
                    string candidatePath = Path.Combine(installation.InstallationPath,
                        Data.Constants.Paths.LaunchboxRelGamesDir, launchboxPlatform.Name);

                    if (Directory.Exists(candidatePath))
                    {
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Warning,
                            $"No Rom Folder set in Launchbox, but folder for this Platform exists in the Launchbox Games folder." +
                            $" Setting to this. It may be wise to check this is the right path.",
                            launchboxPlatform.Name));

                        installationPlatform.ResolvedRomFolder = candidatePath;
                    }
                    else
                    {
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Error,
                            $"No Rom Folder set in Launchbox and no PLatform of this name in Launchbox/Games folder. " +
                            $"You will have to set this manually to enable adding Roms for this Platform.",
                            launchboxPlatform.Name));
                    }
                }
                else if (!Helpers.FileSystem.IsVolumedAndRooted(launchboxPlatform.LaunchboxRomFolder))
                // Rom folder rootless (e.g. "Games\C64 Dreams") ----------------------------------------------------
                {
                    string lbRootRelativePath = Path.Combine(installation.InstallationPath, launchboxPlatform.LaunchboxRomFolder);
                    string lbGamesRelativePath = Path.Combine(installation.InstallationPath, Data.Constants.Paths.LaunchboxRelGamesDir,
                        launchboxPlatform.Name);

                    if (Directory.Exists(lbRootRelativePath))
                    {
                        // If the Rom folder is not rooted, assume it is relative to the installation path
                        installationPlatform.ResolvedRomFolder = lbRootRelativePath;

                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Information,
                            $"Rom Folder path is not rooted: [{launchboxPlatform.LaunchboxRomFolder}]." +
                            $" However, folder exists relative to the Launchbox Root Directory, so set to this:" +
                            $" [{launchboxPlatform.ResolvedRomFolder}]",
                            launchboxPlatform.Name));
                    }
                    else if (Directory.Exists(lbGamesRelativePath))
                    {
                        // If the Rom folder is not rooted, assume it is relative to the installation path
                        installationPlatform.ResolvedRomFolder = lbGamesRelativePath;

                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Information,
                            $"Rom Folder path is not rooted: [{launchboxPlatform.LaunchboxRomFolder}]." +
                            $" However, folder exisits in Launchbox Games Directory, so set to this: [{launchboxPlatform.ResolvedRomFolder}]",
                            launchboxPlatform.Name));
                    }
                    else
                    {
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Error,
                            $"Rom Folder path is not rooted and no folder found for this in the Launchbox Games folder. " +
                            $"You will have to set this manually to add Roms to this Platform. " +
                            $"Launchbox Path: [{launchboxPlatform.LaunchboxRomFolder}].",
                            launchboxPlatform.Name));
                    }
                }
                else if (Helpers.FileSystem.IsVolumedAndRooted(launchboxPlatform.LaunchboxRomFolder) &&
                    installation.OnRemoteMachine)
                // Rom folder has Drive letter, but installation is on a network share  --------------------------------------
                {
                    if (String.IsNullOrWhiteSpace(installation.RemapRomsFrom))
                    {
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(UpdatePlatformMessageType.Error,
                            $"The Installation is on a network path, but the Launchbox Rom path has a drive letter and no remap specified in the Installation. " +
                            $"Cannot set Rom Folder - you will have to do this manually. Launchbox path: {launchboxPlatform.LaunchboxRomFolder}",
                            launchboxPlatform.Name));
                    }
                    else
                    {
                        //string candidateRomFolder = Path.Combine(installation.InstallationPath,
                        //    launchboxPlatform.LaunchboxRomFolder.Replace(installation.RemapRomsFrom,
                        //                                                installation.RemapRomsTo));
                        string candidateRomFolder = launchboxPlatform.LaunchboxRomFolder.Replace(installation.RemapRomsFrom,
                                                          installation.RemapRomsTo);
                        if (!Directory.Exists(candidateRomFolder))
                        {
                            updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(UpdatePlatformMessageType.Error,
                                $"The Installation is on a network path, but the Launchbox Rom path has a drive letter. " +
                                $"The propsective folder constructed by the Installation Rom folder Remap does not exist. " +
                                $"You will have to set this manually. Launchbox path: {launchboxPlatform.LaunchboxRomFolder}",
                                launchboxPlatform.Name));
                        }
                        else
                        {
                            updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(UpdatePlatformMessageType.Information,
                                $"The Installation is on a network path, but the Launchbox Rom path has a drive letter. " +
                                $"The propsective folder constructed by the Installation Rom folder Remap exists, therefore set to that. " +
                                $"You may be advised to check this path: {candidateRomFolder}",
                                launchboxPlatform.Name));

                            installationPlatform.ResolvedRomFolder = candidateRomFolder;
                        }
                    }
                }
                else
                {
                    // Drop-out = Copy path as is  ----------------------------------------------------
                    updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Information,
                            $"Rom folder added as per Launchbox database entry.",
                            launchboxPlatform.Name));
                    installationPlatform.ResolvedRomFolder = launchboxPlatform.LaunchboxRomFolder;
                }

                // MEDIA FOLDERS ===============================================================================================

                HashSet<string> mappableMediaTypes = new HashSet<string>(databaseService.Database.Collections.LbSsMediaTypeMap
                    .FindAll().Select(m => m.LbMediaType.Name));

                foreach (PlatformFolderModel launchboxPlatformFolder in launchboxPlatform.PlatformFolders
                    .Where(lpf => mappableMediaTypes.Contains(lpf.MediaType.Name)))
                {
                    PlatformFolderModel installationPlatformFolder =
                        installationPlatform.PlatformFolders.Where(pf => pf.MediaType.Name == launchboxPlatformFolder.MediaType.Name).FirstOrDefault();

                    if (installationPlatformFolder == null)
                    {
                        // ADD NEW PLATFORM FOLDER
                        // Does not exist, add new to the database
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Information,
                            $"New Platform Media Folder detected for {launchboxPlatform.Name}. Adding platform folder: [{launchboxPlatformFolder.MediaType}] to the database.",
                            launchboxPlatform.Name));
                        installationPlatformFolder = new PlatformFolderModel();
                        installationPlatform.PlatformFolders.Add(installationPlatformFolder);
                    }
                    else if (installationPlatformFolder.Locked)
                    {
                        // Platform Folder is locked - skip
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Warning,
                            $"Platform Media Folder is locked. Skipping update: [{launchboxPlatform.Name}].",
                            installationPlatform.Name));
                        continue;
                    }
                    else
                    {
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Information,
                            $"Platform Folder already in database for {launchboxPlatform.Name}. Updating: [{launchboxPlatformFolder.MediaType}].",
                            installationPlatform.Name));
                    }

                    installationPlatformFolder.MediaType = launchboxPlatformFolder.MediaType;
                    installationPlatformFolder.LaunchboxMediaPath = launchboxPlatformFolder.LaunchboxMediaPath;

                    if (!Helpers.FileSystem.IsVolumedAndRooted(launchboxPlatformFolder.LaunchboxMediaPath))
                    {
                        // Media folder rootless (e.g. "Games\C64 Dreams") ----------------------------------------------------
                        string lbRootRelativePath = Path.Combine(installation.InstallationPath, launchboxPlatformFolder.LaunchboxMediaPath);

                        string folder = "Images";

                        switch (launchboxPlatformFolder.MediaType.Name)
                        {
                            case "Video":
                                folder = "Videos"; break;
                            case "Music":
                                folder = "Music"; break;
                            case "Manual":
                                folder = "Manuals"; break;
                        }
                        string lbMediaRelativePath = Path.Combine(installation.InstallationPath, folder);

                        if (Directory.Exists(lbRootRelativePath))
                        {
                            // If the Media folder is not rooted, assume it is relative to the installation path
                            installationPlatformFolder.ResolvedMediaPath = lbRootRelativePath;

                            updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                                UpdatePlatformMessageType.Information,
                                $"Media Folder path is not rooted: [{launchboxPlatformFolder.LaunchboxMediaPath}]." +
                                $" However, folder exists relative to the Launchbox Root Directory, so set to this:" +
                                $" [{launchboxPlatform.ResolvedRomFolder}]",
                                launchboxPlatform.Name));
                        }
                        else if (Directory.Exists(lbMediaRelativePath))
                        {
                            // If the Rom folder is not rooted, assume it is relative to the installation path
                            installationPlatformFolder.ResolvedMediaPath = lbMediaRelativePath;

                            updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                                UpdatePlatformMessageType.Information,
                                $"Media Folder path is not rooted: [{launchboxPlatformFolder.LaunchboxMediaPath}]." +
                                $" However, folder exisits in Launchbox Directory, so set to this: [{launchboxPlatform.ResolvedRomFolder}]",
                                launchboxPlatform.Name));
                        }
                        else
                        {
                            updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                                UpdatePlatformMessageType.Error,
                                $"Media Folder path is not rooted and no folder found for this in the Launchbox folder. " +
                                $"You will have to set this manually to add Media to this Platform. " +
                                $"Launchbox Path: [{launchboxPlatformFolder.LaunchboxMediaPath}].",
                                launchboxPlatform.Name));
                        }
                    }
                    else if (Helpers.FileSystem.IsVolumedAndRooted(launchboxPlatformFolder.LaunchboxMediaPath) &&
                                installation.OnRemoteMachine)
                    // Media folder has Drive letter, but installation is on a network share  --------------------------------------
                    {
                        if (String.IsNullOrWhiteSpace(installation.RemapMediaFrom))
                        {
                            updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(UpdatePlatformMessageType.Error,
                                $"The Installation is on a network path, but the Launchbox Media path has a drive letter and no remap specified in the Installation. " +
                                $"Cannot set Media Folder - you will have to do this manually. Launchbox path: {launchboxPlatformFolder.LaunchboxMediaPath}",
                                launchboxPlatform.Name));
                        }
                        else
                        {
                            string candidateMediaFolder = launchboxPlatformFolder.LaunchboxMediaPath.Replace(installation.RemapMediaFrom,
                                                                 installation.RemapMediaTo);
                            if (!Directory.Exists(candidateMediaFolder))
                            {
                                updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(UpdatePlatformMessageType.Error,
                                    $"The Installation is on a network path, but the Launchbox Media path has a drive letter. " +
                                    $"The propsective folder constructed by the Installation Media folder Remap does not exist. " +
                                    $"You will have to set this manually. Launchbox path: {launchboxPlatformFolder.LaunchboxMediaPath}",
                                    launchboxPlatform.Name));
                            }
                            else
                            {
                                updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(UpdatePlatformMessageType.Information,
                                    $"The Installation is on a network path, but the Launchbox Rom path has a drive letter. " +
                                    $"The propsective folder constructed by the Installation Rom folder Remap exists, therefore set to that. " +
                                    $"You may be advised to check this path: {candidateMediaFolder}",
                                    launchboxPlatform.Name));

                                installationPlatformFolder.ResolvedMediaPath = candidateMediaFolder;
                            }
                        }
                    }
                    else
                    {
                        // Drop-out = Copy path as is  ----------------------------------------------------
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                                UpdatePlatformMessageType.Information,
                                $"Media folder added as per Launchbox database entry: {launchboxPlatformFolder.LaunchboxMediaPath}",
                                launchboxPlatform.Name));
                        installationPlatformFolder.ResolvedMediaPath = launchboxPlatformFolder.LaunchboxMediaPath;
                    }
                }
            } // END of xmlPlatforms foreach

            // NB: Don't forget to review the xml for any REMOVED Platforms (i.e. local db PLatform.Name cannot be found in the xml)
            List<PlatformModel> platformsToRemove = new List<PlatformModel>();
            foreach (var uniboxPlatforms in installation.Platforms)
            {
                if (!xmlPlatforms.Any(p => p.Name == uniboxPlatforms.Name))
                {
                    updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                        UpdatePlatformMessageType.Information,
                        $"Platform not found in the XML file. Removing: [{uniboxPlatforms.Name}] from the database.",
                        uniboxPlatforms.Name));
                    platformsToRemove.Add(uniboxPlatforms);
                }
            }
            foreach (var platformToRemove in platformsToRemove)
            {
                installation.Platforms.Remove(platformToRemove);
            }

            installationService.Update(installation);

            updatePlatformsOutcome.UpdatePlatformOutcome = UpdatePlatformOutcome.Success;
            return updatePlatformsOutcome;
        }

        private void SendMessage(UpdatePlatformMessageType updatePlatformMessageType, string message)
        {
            WeakReferenceMessenger.Default.Send(new UpdatePlatformMessageDetails()
            {
                MessageType = updatePlatformMessageType,
                SummaryLine = message
            });
        }

        public ObservableCollection<PlatformModel> GetPlatformsFromXml(string installationPath)
        {
            ObservableCollection<PlatformModel> lbPlatforms = new ObservableCollection<PlatformModel>();

            // Load the XML file containing the platforms
            string xmlFilePath = Path.Combine(installationPath, Data.Constants.Paths.LaunchboxRelDataDir,
                Data.Constants.Paths.LaunchboxPlatformsXmlFile);

            if (!File.Exists(xmlFilePath))
            {
                return null; // Return empty list if the file does not exist
            }

            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(xmlFilePath);

            // Parse the XML to extract platform information
            Debug.WriteLine(doc.Root.Elements("Platform").Count());
            foreach (var platformElement in doc.Root.Elements("Platform"))
            {
                PlatformModel platform = new PlatformModel
                {
                    Name = platformElement.Element("Name")?.Value,
                    LaunchboxRomFolder = platformElement.Element("Folder")?.Value,
                    LaunchboxScrapeAs = platformElement.Element("ScrapeAs")?.Value
                };

                platform.PlatformFolders = new ObservableCollection<PlatformFolderModel>();

                foreach (var platformFolderElement in doc.Root.Elements("PlatformFolder").Where(pf => pf.Element("Platform").Value == platform.Name))
                {
                    string lbMediaType = platformFolderElement.Element("MediaType")?.Value;
                    // ⚠️ Digressed here to media/system maps ⚠️
                    if (platformFolderElement.Element("Platform")?.Value == platform.Name &&
                        databaseService.Database.Collections.LbSsMediaTypeMap.Exists(m => m.LbMediaType.Name == lbMediaType))
                    {
                        PlatformFolderModel platformFolder = new PlatformFolderModel
                        {
                            MediaType = databaseService.Database.Collections.LbMediaTypes.Query().Where(m => m.Name == lbMediaType).FirstOrDefault(),
                            LaunchboxMediaPath = platformFolderElement.Element("FolderPath")?.Value,
                        };
                        platform.PlatformFolders.Add(platformFolder);
                    }
                }

                lbPlatforms.Add(platform);
            }

            return lbPlatforms;
        }
    }
}