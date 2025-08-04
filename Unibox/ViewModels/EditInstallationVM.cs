using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Unibox.Data.Enums;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Services;
using Unibox.Views;

namespace Unibox.ViewModels
{
    internal partial class EditInstallationVM : ObservableObject, IDataErrorInfo, IRecipient<UpdatePlatformMessage>
    {
        [ObservableProperty]
        private InstallationModel installation;

        [ObservableProperty]
        private string name = "Atari 1280";

        [ObservableProperty]
        private string installationPath = @"\\ATARI-1280\Users\admin\LaunchBox";

        [ObservableProperty]
        private bool onRemoteMachine = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemapRomsTo))]
        private string remapRomsFrom = @"D:\Games";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemapRomsFrom))]
        private string remapRomsTo = @"\\ATARI-1280\Games";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemapMediaTo))]
        private string remapMediaFrom = @"D:\Assets";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemapMediaFrom))]
        private string remapMediaTo = @"\\ATARI-1280\Assets";

        [ObservableProperty]
        private bool isValid;

        partial void OnInstallationChanged(InstallationModel value)
        {
            UpdateViewModelFromDataObject();
        }

        public string Error => null;

        public Data.Enums.DialogResult DialogResult { get; set; } = Data.Enums.DialogResult.Unset;

        private DatabaseService databaseService;
        private PlatformService platformUpdateService;
        private InstallationService installationService;

        private static readonly string[] ValidatedProperties =
            { nameof(Name), nameof(InstallationPath), nameof(RemapRomsFrom), nameof(RemapRomsTo), nameof(RemapMediaFrom), nameof(RemapMediaTo) };

        [RelayCommand]
        private void BrowseForInstallationPath()
        {
            string path = installationService.GetInstallationPath();
            if (!string.IsNullOrWhiteSpace(path) && installationService.IsLaunchboxRootDirectory(path))
            {
                InstallationPath = path;
                if (!OnRemoteMachine)
                {
                    RemapMediaFrom = string.Empty;
                    RemapMediaTo = string.Empty;
                    RemapRomsFrom = string.Empty;
                    RemapRomsTo = string.Empty;
                }
            }
            else
            {
                AdonisUI.Controls.MessageBox.Show("Invalid path selected for the Launchbox root directory. Please ammend", "Invalid Path",
                     AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void BrowseForRemapTo(string pathType)
        {
            string path = Unibox.Helpers.FileSystem.GetRemapToPath();

            if (String.IsNullOrWhiteSpace(path)) return;

            if (!Helpers.FileSystem.IsNetworkPath(path))
            {
                AdonisUI.Controls.MessageBox.Show("The path must be a UNC network path. Please revise", "Invalid Path",
                     AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                return;
            }

            if (pathType == "Roms")
                RemapRomsTo = path;
            else
                RemapMediaTo = path;
        }

        [RelayCommand]
        private void ClearText(string relevantText)
        {
            if (relevantText == "Roms")
                RemapRomsTo = String.Empty;
            else if (relevantText == "Media")
                RemapMediaTo = String.Empty;
        }

        [RelayCommand]
        private void ShowPlatformRomFolders()
        {
            string prospectiveXmlPath = Path.Combine(InstallationPath, Data.Constants.Paths.LaunchboxRelDataDir,
                Data.Constants.Paths.LaunchboxPlatformsXmlFile);

            if (!File.Exists(prospectiveXmlPath))
            {
                AdonisUI.Controls.MessageBox.Show($"Could not locate the Launchbox platforms.xml file for this installation. " +
                    $"Please check it is available on this path: {prospectiveXmlPath}", "Invalid Path",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                return;
            }

            ObservableCollection<PlatformModel> platforms = new ObservableCollection<PlatformModel>(platformUpdateService.GetPlatformsFromXml(InstallationPath).OrderBy(p => p.Name));

            InstallationPlatformDetails installationPlatformDetails = new InstallationPlatformDetails();

            installationPlatformDetails.ViewModel.PageTitle = "Launchbox database paths for Installation";
            installationPlatformDetails.ViewModel.PageSubtitle = "This list allows you to study what each Platform's Rom and Media paths look like in" +
                " the installation's Launchbox database. Useful when considering whether to set up a path remap for remote installations" +
                " (where rom paths are stored as local paths on the remote machine, meaning you need to remap the relevant part of those" +
                " paths to use the remote UNC path instead).";

            installationPlatformDetails.ViewModel.Platforms = new ObservableCollection<PlatformModel>(platforms);

            installationPlatformDetails.ShowDialog();

            DialogResult dialogResult = installationPlatformDetails.ViewModel.DialogResult;
            if (dialogResult == Data.Enums.DialogResult.UsePath)
            {
                if (installationPlatformDetails.ViewModel.SelectedPlatform != null)
                {
                    RemapRomsFrom = installationPlatformDetails.ViewModel.SelectedPlatform.LaunchboxRomFolder;
                    RemapMediaFrom = installationPlatformDetails.ViewModel.SelectedPlatformFolder.Folderpath;
                }
            }
        }

        [RelayCommand]
        private void ProcessOkButton(Window window)
        {
            AddInstallation();

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

        public EditInstallationVM()
        {
        }

        public EditInstallationVM(DatabaseService databaseService, InstallationService installationService, PlatformService platformUpdateService)
        {
            this.databaseService = databaseService;
            this.installationService = installationService;
            this.platformUpdateService = platformUpdateService;

            Helpers.Theming.ApplyTheme();
        }

        private void UpdateViewModelFromDataObject()
        {
            if (installation != null)
            {
                Name = installation.Name;
                InstallationPath = installation.InstallationPath;
                OnRemoteMachine = installation.OnRemoteMachine;
                RemapRomsFrom = installation.RemapRomsFrom;
                RemapRomsTo = installation.RemapRomsTo;
                RemapMediaFrom = installation.RemapMediaFrom;
                RemapMediaTo = installation.RemapMediaTo;
                ValidateForm();
            }
        }

        private void AddInstallation()
        {
            InstallationModel newInstallation = new InstallationModel()
            {
                Name = Name,
                InstallationPath = InstallationPath,
                OnRemoteMachine = OnRemoteMachine,
                RemapRomsFrom = RemapRomsFrom,
                RemapRomsTo = RemapRomsTo,
                RemapMediaFrom = RemapMediaFrom,
                RemapMediaTo = RemapMediaTo
            };

            var outcome = platformUpdateService.UpdateInstallationPlatforms(newInstallation);

            if (outcome.UpdatePlatformOutcome == UpdatePlatformOutcome.Success)
            {
                //databaseService.Database.Collections.Installations.Insert(newInstallation);
            }
            else
            {
            }
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
                    else if (!installationService.IsUniqueName(Name, Installation))
                        return "Another Installation with this name already exists.";
                    break;

                case nameof(InstallationPath):
                    OnRemoteMachine = Helpers.FileSystem.IsNetworkPath(InstallationPath);
                    if (string.IsNullOrWhiteSpace(InstallationPath))
                        return "Installation path cannot be empty.";
                    else if (databaseService.Database.Collections.Installations.Find(x => x.InstallationPath == InstallationPath).Count() > 1)
                        return "There is already another Installation with this path.";
                    else if (installationService.IsLaunchboxRootDirectory(InstallationPath))
                        return "Not a Launchbox root directory.";
                    break;

                case nameof(RemapRomsFrom):
                    if (!String.IsNullOrWhiteSpace(RemapRomsFrom))
                    {
                        if (!OnRemoteMachine)
                            return "Remap not needed as Installation is on this local machine. Please remove.";
                        else if (!Helpers.FileSystem.IsVolumedAndRooted(RemapRomsFrom))
                            return "Replace Text path must be a Volumed path (e.g. 'D:\\Games')";
                        else if (RemapRomsFrom.EndsWith('/') || RemapRomsFrom.EndsWith('\\'))
                            return "Replace Text path cannot end with a slash or backslash.";
                    }
                    //else if (RemapRomsFrom.Length > 0 && String.IsNullOrWhiteSpace(RemapRomsFrom))
                    //    return "Replace Text path is just whitespace.";
                    break;

                case nameof(RemapRomsTo):
                    if (!String.IsNullOrWhiteSpace(RemapRomsFrom) && String.IsNullOrWhiteSpace(RemapRomsTo))
                        return "Cannot be null when Replace Text is set.";
                    else if (!String.IsNullOrWhiteSpace(RemapRomsTo) && String.IsNullOrWhiteSpace(RemapRomsFrom))
                        return "A path must be set in Replace Text.";
                    else if (!String.IsNullOrWhiteSpace(RemapRomsFrom) && !Helpers.FileSystem.IsNetworkPath(RemapRomsTo))
                        return "The path must be a network path";
                    break;

                case nameof(RemapMediaFrom):
                    if (!String.IsNullOrWhiteSpace(RemapMediaFrom))
                    {
                        if (!OnRemoteMachine)
                            return "Remap not needed as Installation is on this local machine. Please remove.";
                        else if (!Helpers.FileSystem.IsVolumedAndRooted(RemapMediaFrom))
                            return "Replace Text path must be a Volumed path (e.g. 'D:\\Games')";
                        else if (RemapMediaFrom.EndsWith('/') || RemapMediaFrom.EndsWith('\\'))
                            return "Replace Text path cannot end with a slash or backslash.";
                    }
                    //else if (RemapMediaFrom.Length > 0 && String.IsNullOrWhiteSpace(RemapMediaFrom))
                    //    return "Replace Text path is just whitespace.";
                    break;

                case nameof(RemapMediaTo):
                    if (!String.IsNullOrWhiteSpace(RemapMediaFrom) && String.IsNullOrWhiteSpace(RemapMediaTo))
                        return "Cannot be null when Replace Text is set.";
                    else if (!String.IsNullOrWhiteSpace(RemapMediaTo) && String.IsNullOrWhiteSpace(RemapMediaFrom))
                        return "A path must be set in Replace Text.";
                    else if (!String.IsNullOrWhiteSpace(RemapMediaFrom) && !Helpers.FileSystem.IsNetworkPath(RemapMediaTo))
                        return "The path must be a network path";
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