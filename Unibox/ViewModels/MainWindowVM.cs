using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.LiteDb;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Services;

namespace Unibox.ViewModels
{
    public partial class MainWindowVM : ObservableObject, IRecipient<InstallationAddedMessage>
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

            UpdateIstallationsFromDatabase();

            NavigateToInstallations();            
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

        void IRecipient<InstallationAddedMessage>.Receive(InstallationAddedMessage message)
        {
            UpdateIstallationsFromDatabase();
        }
    }
}

