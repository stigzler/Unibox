using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string searchTerm;

        [ObservableProperty]
        private InstallationModel selectedInstallation;

        [ObservableProperty]
        private PlatformModel selectedPlatform;

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
            List<string> romFiles = Helpers.FileSystem.GetFilePaths("Please select the rom file/s to add");

            if (romFiles is null || romFiles.Count == 0)
            {
                AdonisUI.Controls.MessageBox.Show("No Rom files selected. Please select at least one Rom file to add.", "No Rom Files Selected",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                return;
            }

            ObservableCollection<AddGameOutcome> addGameOutcomes = new ObservableCollection<AddGameOutcome>();

            PleaseWaitWindow pleaseWaitWindow = new PleaseWaitWindow();
            pleaseWaitWindow.ViewModel.Text = "Please wait whilst the selected Roms are processed...";
            pleaseWaitWindow.Show();

            int count = 1;

            foreach (string romFile in romFiles)
            {
                WeakReferenceMessenger.Default.Send(new ProgressMessage(
                    new ProgressMessageArgs
                    {
                        PrimaryMessage = $"Processing Rom ({count}/{romFiles.Count()}): {Path.GetFileName(romFile)}",
                        SecondaryMessage = "Starting process."
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

            pleaseWaitWindow.CloseWindow();

            AddGameResultsWindow addGameResultsWindow = new AddGameResultsWindow();
            addGameResultsWindow.ViewModel.AddGameResults = addGameOutcomes;
            addGameResultsWindow.ShowDialog();

            // Update User on outcomes
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

            Mouse.OverrideCursor = Cursors.Wait;
            InstallationAvailable = Directory.Exists(value.InstallationPath);
            Mouse.OverrideCursor = Cursors.Arrow;

            platforms = value.Platforms;
            PlatformsView = (CollectionView)CollectionViewSource.GetDefaultView(platforms);
            PlatformsView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

            games.Clear();
        }

        partial void OnSelectedPlatformChanged(PlatformModel value)
        {
            UpdateGamesList();
            GamesView = (CollectionView)CollectionViewSource.GetDefaultView(games);
            GamesView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void UpdateGamesList()
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
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public void Receive(InstallationChangedMessage message)
        {
            installations = new ObservableCollection<InstallationModel>(databaseService.Database.Collections.Installations.FindAll());
            InstallationsView = (CollectionView)CollectionViewSource.GetDefaultView(installations);
        }
    }
}