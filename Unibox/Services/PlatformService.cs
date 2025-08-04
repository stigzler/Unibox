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
                else
                {
                    updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                        UpdatePlatformMessageType.Information,
                        $"Platform already in database. Updating: [{launchboxPlatform.Name}].",
                        installationPlatform.Name));
                }

                installationPlatform.Name = launchboxPlatform.Name;
                installationPlatform.LaunchboxScrapeAs = launchboxPlatform.LaunchboxScrapeAs;
                installationPlatform.LaunchboxRomFolder = launchboxPlatform.LaunchboxRomFolder;

                // Check LaunchboxScrapeAs
                if (launchboxPlatform.LaunchboxScrapeAs == null)
                {
                    updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                        UpdatePlatformMessageType.Warning,
                        $"No ScrapeAs set in Launchbox. You will have to set this manually to enable metadata/media scrapes.",
                        launchboxPlatform.Name));
                }

                // ROM FOLDER OPS

                if (String.IsNullOrWhiteSpace(launchboxPlatform.LaunchboxRomFolder))
                // Rom Folder Null - try to resolve to LB Games folder
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
                // Rom folder rootless (e.g. "Games\C64 Dreams")
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
                // Rom folder has Drive letter, but installation is on a network share
                {
                    if (installation.RemapRomsFrom == null)
                    {
                        updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(UpdatePlatformMessageType.Error,
                            $"The Installation is on a network path, but the Launchbox Rom path has a drive letter and no remap specified in the Installation. " +
                            $"Cannot set Rom Folder - you will have to do this manually. Launchbox path: {launchboxPlatform.LaunchboxRomFolder}",
                            launchboxPlatform.Name));
                    }
                    else
                    {
                        string candidateRomFolder = Path.Combine(installation.InstallationPath,
                            launchboxPlatform.LaunchboxRomFolder.Replace(installation.RemapRomsFrom,
                                                                        installation.RemapRomsTo));
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
                    updatePlatformsOutcome.SubOperationOutcomes.Add(new UpdatePlatformsSubOperationOutcome(
                            UpdatePlatformMessageType.Information,
                            $"Rom folder added as per Launchbox database entry.",
                            launchboxPlatform.Name));
                    installationPlatform.ResolvedRomFolder = launchboxPlatform.LaunchboxRomFolder;
                }
            } // END of xmlPlatforms foreach

            // NB: Don't forget to review the xml for any REMOVED Platforms (i.e. local db PLatform.Name cannot be found in the xml)

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
                    if (platformFolderElement.Element("Platform")?.Value == platform.Name)
                    {
                        PlatformFolderModel platformFolder = new PlatformFolderModel
                        {
                            MediaType = platformFolderElement.Element("MediaType")?.Value,
                            Folderpath = platformFolderElement.Element("FolderPath")?.Value,
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