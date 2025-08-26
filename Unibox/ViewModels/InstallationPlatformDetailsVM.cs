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
        private string pageTitle = "Launchbox database paths for Installation";

        [ObservableProperty]
        private string pageSubtitle = Properties.Resources.PlatformDetailsText;

        [ObservableProperty]
        private ObservableCollection<PlatformModel> platforms = new ObservableCollection<PlatformModel>();

        [ObservableProperty]
        private ObservableCollection<PlatformFolderModel> platformFolders = new ObservableCollection<PlatformFolderModel>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlatformFolders))]
        private PlatformModel selectedPlatform = null;

        [ObservableProperty]
        private PlatformFolderModel selectedPlatformFolder = null;

        public Data.Enums.DialogResult DialogResult { get; set; }

        public InstallationPlatformDetailsVM()
        {
        }

        partial void OnSelectedPlatformChanged(PlatformModel value)
        {
            platformFolders = value.PlatformFolders;
        }

        [RelayCommand]
        private void UsePath(Window window)
        {
            DialogResult = DialogResult.UsePath;
            Clipboard.SetText(SelectedPlatform?.LaunchboxRomFolder ?? string.Empty);
            if (window != null) window.Close();
        }

        [RelayCommand]
        private void ProcessOkButton(Window window)
        {
            DialogResult = DialogResult.OK;
            if (window != null) window.Close();
        }
    }
}