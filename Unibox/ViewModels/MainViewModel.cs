using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.LiteDb;
using Unibox.Data.Models;
using Unibox.Services;

namespace Unibox.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? currentPage;

        [ObservableProperty]
        private bool isMenuOpen = true;

        [ObservableProperty]
        private IEnumerable<InstallationModel> installations;

        public DatabaseService DatabaseService;

        public MainViewModel()
        {
        }

        public MainViewModel(DatabaseService databaseService)
        {
            DatabaseService = databaseService;
            Helpers.Theming.ApplyTheme();

            Installations = DatabaseService.Database.Collections.Installations.FindAll();

            NavigateToGames();            

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
  
    }
}

