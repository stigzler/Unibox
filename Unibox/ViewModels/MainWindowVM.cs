using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiteDB;
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

namespace Unibox.ViewModels
{
    public partial class MainWindowVM : ObservableObject,
        IRecipient<InstallationAddedMessage>,
        IRecipient<InstallationDeletedMessage>,
        IRecipient<InstallationUpdatedMessage>
    {
        [ObservableProperty]
        private string title = "Unibox";

        [ObservableProperty]
        private object? currentPage;

        [ObservableProperty]
        private bool isMenuOpen = true;

        [ObservableProperty]
        private IEnumerable<InstallationModel> installations;

        public DatabaseService DatabaseService;

        public MainWindowVM()
        {
        }

        public MainWindowVM(DatabaseService databaseService)
        {
            DatabaseService = databaseService;

            Helpers.Theming.ApplyTheme();

            WeakReferenceMessenger.Default.Register<InstallationAddedMessage>(this);
            WeakReferenceMessenger.Default.Register<InstallationDeletedMessage>(this);
            WeakReferenceMessenger.Default.Register<InstallationUpdatedMessage>(this);

            UpdateIstallationsFromDatabase();

            NavigateToInstallations();

            // TEsts

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Unibox.secrets.ini";
            string streamString = null;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                streamString = reader.ReadToEnd();
            }
            Debug.WriteLine(streamString);
        }

        [RelayCommand]
        private void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
        }

        [RelayCommand]
        private void NavigateToGames()
        {
            CurrentPage = new Views.GamesPage();
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            CurrentPage = new Views.SettingsPage();
        }

        [RelayCommand]
        private void NavigateToInstallations()
        {
            CurrentPage = new Views.InstallationsPage();
        }

        private void UpdateIstallationsFromDatabase()
        {
            Installations = new ObservableCollection<InstallationModel>(DatabaseService.Database.Collections.Installations.FindAll());
        }

        void IRecipient<InstallationDeletedMessage>.Receive(InstallationDeletedMessage message)
        {
            UpdateIstallationsFromDatabase();
        }

        void IRecipient<InstallationAddedMessage>.Receive(InstallationAddedMessage message)
        {
            UpdateIstallationsFromDatabase();
        }

        void IRecipient<InstallationUpdatedMessage>.Receive(InstallationUpdatedMessage message)
        {
            UpdateIstallationsFromDatabase();
        }
    }
}