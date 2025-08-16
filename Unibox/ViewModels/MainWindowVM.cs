using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiteDB;
using stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.LiteDb;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Services;
using Unibox.Views;

namespace Unibox.ViewModels
{
    public partial class MainWindowVM : ObservableObject, IRecipient<ProgressMessage>
    {
        [ObservableProperty]
        private string title = "Unibox";

        [ObservableProperty]
        private object? currentPage;

        [ObservableProperty]
        private bool isMenuOpen = true;

        [ObservableProperty]
        private IEnumerable<InstallationModel> installations;

        [ObservableProperty]
        private string logoText = "Unibox";

        [ObservableProperty]
        private string statusBarText = "No current Operation.";

        [ObservableProperty]
        private bool statusBarProgressBarIndeterminate = false;

        [ObservableProperty]
        private int statusBarProgressBarPercentage = 0;

        public DatabaseService DatabaseService;

        private SettingsPage settingsPage = new SettingsPage();
        private GamesPage gamesPage = new GamesPage();
        private InstallationsPage installationsPage = new InstallationsPage();

        public MainWindowVM()
        {
        }

        public MainWindowVM(DatabaseService databaseService)
        {
            DatabaseService = databaseService;

            Helpers.Theming.ApplyTheme();

            WeakReferenceMessenger.Default.Register<ProgressMessage>(this);

            UpdateIstallationsFromDatabase();

            NavigateToGames();

            // Testsrefredsss

            var localVersion = Helpers.Plugin.GetApplicationPluginVersion();
        }

        [RelayCommand]
        private void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
        }

        [RelayCommand]
        private void NavigateToGames()
        {
            LogoText = "Games";
            CurrentPage = gamesPage;
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            LogoText = "Settings";
            CurrentPage = settingsPage;
        }

        [RelayCommand]
        private void NavigateToInstallations()
        {
            LogoText = "Installations";
            CurrentPage = installationsPage;
        }

        private void UpdateIstallationsFromDatabase()
        {
            Installations = new ObservableCollection<InstallationModel>(DatabaseService.Database.Collections.Installations.FindAll());
        }

        void IRecipient<ProgressMessage>.Receive(ProgressMessage message)
        {
            StatusBarText = message.Value.PrimaryMessage;

            if (message.Value.ProgressBarIndeterminate)
            {
                StatusBarProgressBarIndeterminate = true;
            }

            if (!String.IsNullOrWhiteSpace(message.Value.SecondaryMessage))
            {
                StatusBarText += " | " + message.Value.SecondaryMessage;
            }

            if (message.Value.PercentageComplete >= 0)
            {
                StatusBarProgressBarIndeterminate = false;
                StatusBarProgressBarPercentage = message.Value.PercentageComplete;
            }
        }
    }
}