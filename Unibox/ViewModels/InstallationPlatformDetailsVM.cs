using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unibox.Data.Enums;
using Unibox.Data.Models;

namespace Unibox.ViewModels
{
    [ObservableObject]
    public partial class InstallationPlatformDetailsVM
    {
        [ObservableProperty]
        private string pageTitle = "Installation Platforms";

        [ObservableProperty]
        private string pageSubtitle = "Details regarding platforms";

        [ObservableProperty]
        ObservableCollection<PlatformModel> platforms = new ObservableCollection<PlatformModel>();

        [ObservableProperty]
        private PlatformModel selectedPlatform = null;

        public InstallationPlatformDetailsVM()
        {
            
        }

        [RelayCommand]
        private void CopyPath()
        {
            Clipboard.SetText(SelectedPlatform?.LaunchboxRomFolder ?? string.Empty);
        }

        [RelayCommand]
        private void ProcessOkButton(Window window)
        {
            if (window != null) window.Close();        
        }

        //public InstallationPlatformDetailsVM(string title, string subtitle, ObservableCollection<PlatformModel> platforms)
        //{

        //}


    }
}
