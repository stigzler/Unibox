using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Unibox.Data.Models;
using Unibox.Data.ServiceOperationOutcomes;
using Unibox.Messages;
using Unibox.Messages.MessageDetails;
using Unibox.Services;
using Unibox.Views;

namespace Unibox.ViewModels
{
    internal partial class GamesVM : ObservableObject, IRecipient<InstallationChangedMessage>
    {
        private DatabaseService databaseService;
        private GameService gameService;

        private ObservableCollection<GameModel> games = new ObservableCollection<GameModel>();

        [ObservableProperty]
        private CollectionView gamesView;

        [ObservableProperty]
        private bool installationAvailable = false;

        private ObservableCollection<InstallationModel> installations;

        [ObservableProperty]
        private CollectionView installationsView;

        private ObservableCollection<PlatformModel> platforms;

        [ObservableProperty]
        private CollectionView platformsView;

        [ObservableProperty]
        private bool canAddRoms = false;

        [ObservableProperty]
        private string searchTerm;

        [ObservableProperty]
        private InstallationModel selectedInstallation;

        [ObservableProperty]
        private PlatformModel selectedPlatform;

        [ObservableProperty]
        private GameModel selectedGame;

        public GamesVM(DatabaseService databaseService, GameService gameService)
        {
            this.databaseService = databaseService;
            this.gameService = gameService;

            installations = new ObservableCollection<InstallationModel>(databaseService.Database.Collections.Installations.FindAll());
            InstallationsView = (CollectionView)CollectionViewSource.GetDefaultView(installations);

            WeakReferenceMessenger.Default.Register<InstallationChangedMessage>(this);
        }

        public GamesVM()
        {
        }

        [RelayCommand]
        private async Task AddRom()
        {
            // Checks:
            // Platform Selected
            if (SelectedPlatform is null)
            {
                AdonisUI.Controls.MessageBox.Show("Please select a platform first.", "No Platform Selected",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                return;
            }

            // Selected platform has a roms folder
            if (String.IsNullOrWhiteSpace(SelectedPlatform.ResolvedRomFolder))
            {
                AdonisUI.Controls.MessageBox.Show("The selected PLatform does not have a Rom Folder set. Please set this via the Installations page.", "No Rom Folder for platform",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                return;
            }

            // Rom folder is accessible
            if (!Directory.Exists(SelectedPlatform.ResolvedRomFolder))
            {
                AdonisUI.Controls.MessageBox.Show($"The folder set for these roms either doesn't exist or is inaccessible. " +
                    $"Please check: \r\n\r\n [{SelectedPlatform.ResolvedRomFolder}]", "Could not access Rom folder",
                AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                return;
            }

            // xml file for Platform exists
            string candidateXmlPath = Path.Combine(SelectedInstallation.InstallationPath, @"Data\Platforms",
                    SelectedPlatform.Name + ".xml");
            if (!File.Exists(candidateXmlPath))
            {
                AdonisUI.Controls.MessageBox.Show(
                    $"The selected platform does not have a valid XML file. Please check the installation path and try again. " +
                    $"Unibox tried: {candidateXmlPath}",
                    "Invalid Platform", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                return;
            }

            // START ADD ROM PROCESS

            // Get rom file/s to add
            List<string> romFiles = Helpers.FileSystem.GetRomFilePaths("Please select the rom file/s to add");

            if (romFiles is null || romFiles.Count == 0)
            {
                AdonisUI.Controls.MessageBox.Show("No Rom files selected. Please select at least one Rom file to add.", "No Rom Files Selected",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                return;
            }

            ObservableCollection<AddGameOutcome> addGameOutcomes = new ObservableCollection<AddGameOutcome>();

            WeakReferenceMessenger.Default.Send(new SetNavigationEnabledMessage(false));

            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.PleaseWait,
                Data = "Please wait whilst the selected Roms are processed..."
            }));

            int count = 1;

            foreach (string romFile in romFiles)
            {
                WeakReferenceMessenger.Default.Send(new ProgressMessage(
                    new ProgressMessageArgs
                    {
                        PrimaryMessage = $"Processing Rom ({count}/{romFiles.Count()}): {Path.GetFileName(romFile)}",
                        SecondaryMessage = "Starting process.",
                        PercentageComplete = (count * 100) / romFiles.Count()
                    }
                ));

                string candidateFilePath = Path.Combine(SelectedPlatform.ResolvedRomFolder, Path.GetFileName(romFile));
                if (File.Exists(candidateFilePath))
                {
                    addGameOutcomes.Add(new AddGameOutcome
                    {
                        RomPath = romFile,
                        Outcomes = new List<string> { "Rom already exists in folder. Not adding." },
                    });
                    continue;
                }

                // >>>>>>>>>>>>>>>>>>>>>> ADDROM <<<<<<<<<<<<<<<<<<<<<<<<<<<<
                var addGameOutcome = await gameService.AddRoms(candidateXmlPath, SelectedPlatform.ResolvedRomFolder, romFile, SelectedPlatform, SelectedInstallation);

                addGameOutcome.RomPath = romFile;

                addGameOutcomes.Add(addGameOutcome);

                if (addGameOutcome.RomAdded) games.Add(addGameOutcome.Game);
                count += 1;
            }

            //pleaseWaitWindow.CloseWindow();

            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.AddGameResults,
                Data = addGameOutcomes
            }));

            WeakReferenceMessenger.Default.Send(new SetNavigationEnabledMessage(true));
        }

        partial void OnSearchTermChanged(string value)
        {
            if (SelectedInstallation is null || SelectedPlatform is null) return;

            GamesView.Filter = game =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    return true;
                GameModel gameModel = game as GameModel;
                return gameModel.Title.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0 ||
                       gameModel.ApplicationPath.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
            };
        }

        partial void OnSelectedInstallationChanged(InstallationModel value)
        {
            if (value == null) return;
            CanAddRoms = false;
            // wait cursor due to potential waits on non-available network paths
            Mouse.OverrideCursor = Cursors.Wait;
            InstallationAvailable = Directory.Exists(value.InstallationPath);
            if (!InstallationAvailable)
            {
                SelectedPlatform = null;
                Mouse.OverrideCursor = Cursors.Arrow;
                return;
            }

            Mouse.OverrideCursor = Cursors.Arrow;

            // now check if latest version of plugin installed on remote launchbox installation
            if (!Helpers.Plugin.IsUniboxPluginInstalled(value))
            {
                var result = AdonisUI.Controls.MessageBox.Show(
                   $"Unibox requires a plugin to be running on the target installation and has detected it requires installing. " +
                   $"Unibox cannot operate without this plugin. Would you like to install this now? Please close any instances of Launchbox or Bigbox on the target installation before doing so.",
                   "Plugin Installation Required", AdonisUI.Controls.MessageBoxButton.YesNo, AdonisUI.Controls.MessageBoxImage.Warning);
                if (result == AdonisUI.Controls.MessageBoxResult.Yes)
                {
                    Exception install = Helpers.Plugin.UpdatePlugin(value);
                    if (install is null)
                    {
                        AdonisUI.Controls.MessageBox.Show(
                           $"The plugin was installed successfully.",
                           "Plugin Installed Successfully", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);
                    }
                    else
                    {
                        AdonisUI.Controls.MessageBox.Show(
                            $"Unibox couldn't install the plugin. Returned exception: {install.Message}",
                            "Error installing plugin", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                        SelectedInstallation = null;
                        return;
                    }
                }
                else
                {
                    SelectedInstallation = null;
                    return;
                }
            }
            else if (Helpers.Plugin.PluginRequiresUpdating(value))
            {
                var result = AdonisUI.Controls.MessageBox.Show(
                       $"The Unibox plugin on the selected installation requires an update. It is STRONGLY recommended that this is done now as " +
                       $"there is a risk of the universe ending with mismatched versions (and that's bad). " +
                       $" Would you like to update the plugin now? Please close any instances of Launchbox or Bigbox on the target installation before doing so.",
                       "Unibox Plugin requires update", AdonisUI.Controls.MessageBoxButton.YesNo, AdonisUI.Controls.MessageBoxImage.Warning);
                if (result == AdonisUI.Controls.MessageBoxResult.Yes)
                {
                    Exception update = Helpers.Plugin.UpdatePlugin(value);
                    if (update is null)
                    {
                        AdonisUI.Controls.MessageBox.Show(
                           $"The plugin was updated successfully.",
                           "Plugin Updated Successfully", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);
                    }
                    else
                    {
                        AdonisUI.Controls.MessageBox.Show(
                            $"Unibox couldn't update the plugin. Returned exception: {update.Message}",
                            "Error updating plugin", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                        SelectedInstallation = null;
                        return;
                    }
                }
                else
                {
                    SelectedInstallation = null;
                    return;
                }
            }

            CanAddRoms = true;

            platforms = value.Platforms;
            PlatformsView = (CollectionView)CollectionViewSource.GetDefaultView(platforms);
            PlatformsView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

            games.Clear();
        }

        partial void OnSelectedPlatformChanged(PlatformModel value)
        {
            UpdateGamesList();
        }

        internal void UpdateGamesList()
        {
            if (SelectedPlatform is null) return;

            string candidateXmlPath = Path.Combine(SelectedInstallation.InstallationPath, @"Data\Platforms",
                SelectedPlatform.Name + ".xml");

            if (!File.Exists(candidateXmlPath))
            {
                AdonisUI.Controls.MessageBox.Show(
                    $"The selected platform does not have a valid XML file. Please check the installation path and try again. " +
                    $"Unibox tried: {candidateXmlPath}",
                    "Invalid Platform", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;
            games = gameService.GetGamesFromXml(candidateXmlPath);

            GamesView = (CollectionView)CollectionViewSource.GetDefaultView(games);
            GamesView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public void Receive(InstallationChangedMessage message)
        {
            var selectedInstallation = SelectedInstallation;
            var selectedPlatform = SelectedPlatform;

            installations = new ObservableCollection<InstallationModel>(databaseService.Database.Collections.Installations.FindAll());
            InstallationsView = (CollectionView)CollectionViewSource.GetDefaultView(installations);

            // hacky, but meh
            if (selectedInstallation != null) SelectedInstallation = installations.Where(i => i.ID == selectedInstallation.ID).FirstOrDefault(); ;
            if (selectedPlatform != null) SelectedPlatform = platforms.Where(p => p.ID == selectedPlatform.ID).FirstOrDefault();
        }

        [RelayCommand]
        private void EditGame()
        {
            if (SelectedGame == null) return;

            SelectedGame.Platform = SelectedPlatform;

            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.EditGame,
                Data = new EditGameMessageDetails()
                {
                    Game = SelectedGame,
                    Installation = SelectedInstallation
                }
            }));
        }

        [RelayCommand]
        private void LaunchPlatformApplication()
        {
            try
            {
                if (SelectedPlatform is null || SelectedInstallation is null) return;
                if (String.IsNullOrWhiteSpace(SelectedPlatform.ApplicationPath) || !File.Exists(SelectedPlatform.ApplicationPath))
                {
                    AdonisUI.Controls.MessageBox.Show("The selected platform does not have a valid application path set. Please check the platform settings.", "Invalid Application Path",
                        AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                    return;
                }
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = SelectedPlatform.ApplicationPath,
                    WorkingDirectory = Path.GetDirectoryName(SelectedPlatform.ApplicationPath),
                };
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                AdonisUI.Controls.MessageBox.Show($"Unibox couldn't launch the application. Exception: {ex.Message}", "Error Launching Application",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void OpenPlatformUrl()
        {
            try
            {
                if (SelectedPlatform is null || SelectedInstallation is null) return;
                Process.Start(new ProcessStartInfo() { FileName = SelectedPlatform.Url, UseShellExecute = true }); ;
            }
            catch (Exception ex)
            {
                AdonisUI.Controls.MessageBox.Show($"Unibox couldn't open the URL. Exception: {ex.Message}", "Error Opening URL",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
            }
        }
    }
}