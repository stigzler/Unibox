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
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Unibox.Data.LiteDb;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Messages.MessageDetails;
using Unibox.Properties;
using Unibox.Services;
using Unibox.Views;

namespace Unibox.ViewModels
{
    public partial class MainWindowVM : ObservableObject, IRecipient<ProgressMessage>, IRecipient<PageChangeMessage>
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
        private EditInstallationPage editInstallationPage = new EditInstallationPage();
        private EditInstallationPlatformsPage editInstallationPlatformsPage = new EditInstallationPlatformsPage();
        private LoggingService loggingService;

        public MainWindowVM()
        {
        }

        public MainWindowVM(DatabaseService databaseService, LoggingService loggingService)
        {
            DatabaseService = databaseService;
            this.loggingService = loggingService;

            IsMenuOpen = Settings.Default.AppNavBarOpen;

            Helpers.Theming.ApplyTheme();

            WeakReferenceMessenger.Default.Register<ProgressMessage>(this);
            WeakReferenceMessenger.Default.Register<PageChangeMessage>(this);

            UpdateIstallationsFromDatabase();

            NavigateToGames();

            loggingService.StartLog();

            var localVersion = Helpers.Plugin.GetApplicationPluginVersion();
        }

        [RelayCommand]
        private void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
            Settings.Default.AppNavBarOpen = IsMenuOpen;
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

        void IRecipient<PageChangeMessage>.Receive(PageChangeMessage message)
        {
            LogoText = "Installations";

            PageChangeMessageArgs args = (PageChangeMessageArgs)message.Value;

            switch (args.RequestType)
            {
                case Data.Enums.PageRequestType.EditInstallation:
                    InstallationEditMessageDetails msgDetails = (InstallationEditMessageDetails)args.Data;
                    if (msgDetails != null)
                    {
                        if (msgDetails.Installation != null)
                            editInstallationPage.ViewModel.Installation = msgDetails.Installation;

                        if (!String.IsNullOrEmpty(msgDetails.RomsReplaceText))
                            editInstallationPage.ViewModel.RemapRomsFrom = msgDetails.RomsReplaceText;

                        if (!String.IsNullOrEmpty(msgDetails.MediaReplaceText))
                            editInstallationPage.ViewModel.RemapMediaFrom = msgDetails.MediaReplaceText;
                    }

                    CurrentPage = editInstallationPage;
                    break;

                case Data.Enums.PageRequestType.Installations:
                    CurrentPage = installationsPage;
                    break;

                case Data.Enums.PageRequestType.InstallationPlatforms:
                    editInstallationPlatformsPage.ViewModel.Platforms = (ObservableCollection<PlatformModel>?)args.Data;
                    CurrentPage = editInstallationPlatformsPage;
                    break;
            }
        }
    }
}