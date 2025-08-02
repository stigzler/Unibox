using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Services;
using Unibox.Views;

namespace Unibox.ViewModels
{
    internal partial class AddInstallationVM : ObservableObject, IDataErrorInfo, IRecipient<UpdatePlatformMessage>
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string installationPath = @"\\ATARI-1280\Users\admin\LaunchBox";

        [ObservableProperty]
        private bool onRemoteMachine = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemapTo))]
        private string remapFrom = String.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemapFrom))]
        private string remapTo = String.Empty;

        [ObservableProperty]
        private bool isValid;
        public string Error => null;

        public Data.Enums.DialogResult DialogResult { get; set; } = Data.Enums.DialogResult.Unset;

        private DatabaseService databaseService;
        private PlatformUpdateService platformUpdateService;

        private static readonly string[] ValidatedProperties =
            { nameof(Name), nameof(InstallationPath), nameof(RemapFrom), nameof(RemapTo) };

        [RelayCommand]
        private void BrowseForInstallationPath()
        {
            string path = Helpers.FileSystem.GetInstallationsPath();
            if (!string.IsNullOrWhiteSpace(path))
            {
                InstallationPath = path;
            }
        }

        [RelayCommand]
        private void BrowseForRemapTo()
        {
            string path = Unibox.Helpers.FileSystem.GetRemapToPath();

            if (String.IsNullOrWhiteSpace(path)) return;

            if (!Helpers.FileSystem.IsNetworkPath(path))
            {
                AdonisUI.Controls.MessageBox.Show("The path must be a UNC network path. Please revise", "Invalid Path",
                     AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                return;
            }

            RemapTo = path;
        }

        [RelayCommand]
        private void ShowPlatformRomFolders()
        {
            string prospectiveXmlPath = Path.Combine(InstallationPath,Data.Constants.Paths.LaunchboxRelDataDir,
                Data.Constants.Paths.LaunchboxPlatformsXmlFile);

            if (!File.Exists(prospectiveXmlPath))
            {
                AdonisUI.Controls.MessageBox.Show($"Could not locate the Launchbox platforms.xml file for this installation. " +
                    $"Please check it is available on this path: {prospectiveXmlPath}", "Invalid Path",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
            }

            ObservableCollection<PlatformModel> platforms = new ObservableCollection<PlatformModel>(platformUpdateService.GetPlatformsFromXml(InstallationPath).OrderBy(p => p.Name));

            InstallationPlatformDetails installationPlatformDetails =
                new InstallationPlatformDetails();
            installationPlatformDetails.ViewModel.PageTitle = "Launchbox database paths for Installation";
            installationPlatformDetails.ViewModel.PageSubtitle = "This list allows you to study what each Platform's Rom and Media paths look like in" +
                " the installation's Launchbox database. Useful when considering whether to set up a path remap for remote installations" +
                " (where rom paths are stored as local paths on the remote machine, meaning you need to remap the relevant part of those" +
                " paths to use the remote UNC path instead).";
            installationPlatformDetails.ViewModel.Platforms = new ObservableCollection<PlatformModel>(platforms);

            installationPlatformDetails.ShowDialog();

        }

        [RelayCommand]
        private void ProcessOkButton(Window window)
        {
            if (window != null)
            {
                DialogResult = Data.Enums.DialogResult.OK;
                window.Close();
            }
        }

        [RelayCommand]
        private void ProcessCancelButton(Window window)
        {
            if (window != null)
            {
                DialogResult = Data.Enums.DialogResult.Cancel;
                window.Close();
            }
        }




        public AddInstallationVM()
        {
        }

        public AddInstallationVM(DatabaseService databaseService, PlatformUpdateService platformUpdateService)
        {
            this.databaseService = databaseService;
            this.platformUpdateService = platformUpdateService;
            Helpers.Theming.ApplyTheme();
        }


        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string validaitonError = ProcessPropertyChange(propertyName);
                ValidateForm();
                return validaitonError;
            }
        }

        private string ProcessPropertyChange(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                        return "Installation name cannot be empty.";
                    if (databaseService.Database.Collections.Installations.FindOne(x => x.Name == Name) != null)
                        return "An Installation with this name already exists.";
                    break;

                case nameof(InstallationPath):
                    OnRemoteMachine = Helpers.FileSystem.IsNetworkPath(InstallationPath);
                    if (string.IsNullOrWhiteSpace(InstallationPath))
                        return "Installation path cannot be empty.";
                    if (databaseService.Database.Collections.Installations.FindOne(x => x.InstallationPath == InstallationPath) != null)
                        return "There is already an Installation for this path.";
                    if (!Helpers.FileSystem.IsLaunchboxRootDirectory(InstallationPath))
                        return "Not a Launchbox root directory.";
                    break;

                case nameof(RemapFrom):
                    if (!String.IsNullOrWhiteSpace(RemapFrom))
                        if (!OnRemoteMachine)
                            return "Remap not needed as Installation is on this local machine. Please remove.";
                        else if (!Helpers.FileSystem.IsVolumedAndRooted(RemapFrom))
                            return "Remap From path must be a Volumed path (e.g. 'D:\\Games')";
                    else if (RemapFrom.EndsWith('/') || RemapFrom.EndsWith('\\'))
                            return "Remap From path cannot end with a slash or backslash.";
                    else if (Helpers.FileSystem.PathHasInvalidChars(RemapFrom))
                        return "Remap From path cannot contain invalid characters.";
                    break;

                case nameof(RemapTo):
                    if (!String.IsNullOrWhiteSpace(RemapFrom) && String.IsNullOrWhiteSpace(RemapTo))
                        return "Cannot be null when Remap From is set.";

                    if (!String.IsNullOrWhiteSpace(RemapTo) && String.IsNullOrWhiteSpace(RemapFrom))
                        return "A path must be set in Remap To.";
                    break;
            }

            return null;
        }


        public void ValidateForm()
        {

            foreach (string property in ValidatedProperties)
            {
                if (ProcessPropertyChange(property) != null)
                {
                    IsValid = false;
                    return;
                }
            }
            IsValid = true;
        }

        public void Receive(UpdatePlatformMessage message)
        {
            Debug.WriteLine(message);
        }
    }
}

