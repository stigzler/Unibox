using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unibox.Data.Enums;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Messages.MessageDetails;

namespace Unibox.ViewModels
{
    [ObservableObject]
    public partial class EditInstallationPlatformsVM
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

        public EditInstallationPlatformsVM()
        {
        }

        partial void OnSelectedPlatformChanged(PlatformModel value)
        {
            if (value == null) return;
            platformFolders = value.PlatformFolders;
        }

        [RelayCommand]
        private void UsePath(Window window)
        {
            if (SelectedPlatform == null || selectedPlatformFolder == null)
            {
                AdonisUI.Controls.MessageBox.Show($"Select both a Platform and Platform Media Folder in order to use paths.",
                       "Platform and Media folder required", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                return;
            }

            DialogResult = DialogResult.UsePath;
            Clipboard.SetText(SelectedPlatform?.LaunchboxRomFolder ?? string.Empty);
            if (window != null) window.Close();

            InstallationEditMessageDetails msgDetails = new InstallationEditMessageDetails()
            {
                Installation = null,
                RomsReplaceText = SelectedPlatform.LaunchboxRomFolder,
                MediaReplaceText = SelectedPlatformFolder.LaunchboxMediaPath
            };

            WeakReferenceMessenger.Default.Send(new PageChangeMessage(PageRequestType.EditInstallation,
                msgDetails));
        }

        [RelayCommand]
        private void GoBack(Window window)
        {
            WeakReferenceMessenger.Default.Send(new PageChangeMessage(PageRequestType.EditInstallation));
        }
    }
}