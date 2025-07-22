using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? currentPage;

        [ObservableProperty]
        private bool isMenuOpen = true;

        public MainViewModel()
        {
            Helpers.Theming.ApplyTheme();
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

