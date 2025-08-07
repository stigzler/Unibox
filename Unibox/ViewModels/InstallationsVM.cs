﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Services;
using Unibox.Views;

namespace Unibox.ViewModels
{
    internal partial class InstallationsVM : ObservableObject, IRecipient<InstallationUpdatedMessage>
    {
        [ObservableProperty]
        private Cursor cursor = Cursors.Arrow;

        private DatabaseService databaseService;

        [ObservableProperty]
        private ObservableCollection<InstallationModel> installations = new ObservableCollection<InstallationModel>();

        private InstallationService installationService;

        [ObservableProperty]
        private ObservableCollection<PlatformFolderModel> platformFolders = new ObservableCollection<PlatformFolderModel>();

        [ObservableProperty]
        private ObservableCollection<PlatformModel> platforms = new ObservableCollection<PlatformModel>();

        private PlatformService platformService;

        [ObservableProperty]
        private InstallationModel selectedInstallation;

        [ObservableProperty]
        private PlatformModel selectedPlatform;

        [ObservableProperty]
        private List<PlatformModel> selectedPlatforms;

        [ObservableProperty]
        private PlatformFolderModel selectedPlatformFolder;

        public InstallationsVM()
        {
        }

        public InstallationsVM(DatabaseService databaseService, InstallationService installationService, PlatformService platformService)
        {
            this.databaseService = databaseService;
            this.installationService = installationService;
            this.platformService = platformService;

            WeakReferenceMessenger.Default.Register<InstallationUpdatedMessage>(this);

            UpdateIstallationsFromDatabase();
            this.platformService = platformService;
        }

        void IRecipient<InstallationUpdatedMessage>.Receive(InstallationUpdatedMessage message)
        {
            UpdateIstallationsFromDatabase();
        }

        [RelayCommand]
        private void UpdateRomFolder()
        {
            if (selectedPlatform == null) return;

            string prospectivePath = Helpers.FileSystem.GetFolderPath($"Please select the Rom Folder to use for the Platform: {selectedPlatform.Name}",
                SelectedPlatform.ResolvedRomFolder);
            if (prospectivePath == String.Empty) return;
            SelectedPlatform.ResolvedRomFolder = prospectivePath;
            databaseService.Database.Collections.Installations.Update(SelectedInstallation);
            UpdatePlatformsList();
        }

        [RelayCommand]
        private void UpdatePlatformMediaFolder()
        {
            if (selectedPlatformFolder == null) return;
            string prospectivePath = Helpers.FileSystem.GetFolderPath($"Please select the Media Folder to use for this Media Type: {selectedPlatformFolder.MediaType}",
                SelectedPlatformFolder.ResolvedMediaPath);
            if (prospectivePath == String.Empty) return;
            SelectedPlatformFolder.ResolvedMediaPath = prospectivePath;
            databaseService.Database.Collections.Installations.Update(SelectedInstallation);
            UpdatePlatformFoldersList();
        }

        [RelayCommand]
        private void AddNewInstallation()
        {
            string prospectivePath = installationService.GetInstallationPath();
            if (prospectivePath == String.Empty) return;

            if (!installationService.IsLaunchboxRootDirectory(prospectivePath))
            {
                AdonisUI.Controls.MessageBox.Show($"The path selected is not a Launchbox root path. New Installation not created.", "Invalid Path",
                        AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                return;
            }
            else if (!installationService.IsUniqueInstallationPath(prospectivePath))
            {
                AdonisUI.Controls.MessageBox.Show($"There is already an Installation created for this Launchbox location. New Installation not created.", "Invalid Path",
                        AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                return;
            }

            InstallationModel newInstallation = installationService.AddNew(prospectivePath);

            if (newInstallation != null)
            {
                AdonisUI.Controls.MessageBox.Show($"New Installation added successfully. " +
                    $"If this is for a remote installation, it is strongly advised to edit it and set up Remote Path Modificiations " +
                    $"to ensure that Platforms get imported correctly. Once done, or if this is a local installation, you can " +
                    $"now proceed to Update Platforms below.",
                    "New Installaiton created",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);
                WeakReferenceMessenger.Default.Send(new Messages.InstallationAddedMessage(newInstallation));
                UpdateIstallationsFromDatabase();
            }

            //UpdateIstallationsFromDatabase();

            //databaseService.Database.Collections.Installations.Insert(newInstallation);
        }

        [RelayCommand]
        private void DeleteInstallation()
        {
            if (selectedInstallation == null) return;

            if (AdonisUI.Controls.MessageBox.Show($"Are you sure you want to delete the installation: {selectedInstallation.Name}?",
                       "Delete Installation", AdonisUI.Controls.MessageBoxButton.YesNo, AdonisUI.Controls.MessageBoxImage.Warning)
                       == AdonisUI.Controls.MessageBoxResult.Yes)
            {
                installationService.Delete(selectedInstallation);
                WeakReferenceMessenger.Default.Send(new InstallationDeletedMessage(selectedInstallation));
                UpdateIstallationsFromDatabase();
            }
        }

        [RelayCommand]
        private void EditInstallation()
        {
            if (selectedInstallation == null) return;

            EditInstallationWindow editInstallationWindow = new EditInstallationWindow();
            editInstallationWindow.ViewModel.Installation = SelectedInstallation;
            editInstallationWindow.ShowDialog();
        }

        partial void OnSelectedInstallationChanged(InstallationModel value)
        {
            Platforms = null;
            PlatformFolders = null;
            //SelectedPlatformFolder = null;

            UpdatePlatformsList();
        }

        partial void OnSelectedPlatformChanged(PlatformModel value)
        {
            UpdatePlatformFoldersList();
        }

        private void UpdateIstallationsFromDatabase()
        {
            Installations = installationService.GetAllInstallations();
        }

        private void UpdatePlatformFoldersList()
        {
            if (SelectedPlatform == null) return;
            PlatformFolders = new ObservableCollection<PlatformFolderModel>(SelectedPlatform.PlatformFolders);
        }

        [RelayCommand]
        private void UpdatePlatforms()
        {
            if (selectedInstallation == null) return;

            Cursor = Cursors.Wait;

            platformService.UpdateInstallationPlatforms(selectedInstallation);

            UpdatePlatformsList();

            Cursor = Cursors.Arrow;
        }

        [RelayCommand]
        private void TogglePlatformLocks(IList platforms)
        {
            foreach (var platform in platforms.OfType<PlatformModel>())
            {
                platform.Locked = !platform.Locked;
                databaseService.Database.Collections.Installations.Update(selectedInstallation);
            }
            UpdatePlatformsList();
        }

        [RelayCommand]
        private void ToggleMediaFolderLocks(IList mediaFolders)
        {
            foreach (var mediaFolder in mediaFolders.OfType<PlatformFolderModel>())
            {
                mediaFolder.Locked = !mediaFolder.Locked;
                databaseService.Database.Collections.Installations.Update(selectedInstallation);
            }
            UpdatePlatformFoldersList();
        }

        private void UpdatePlatformsList()
        {
            if (SelectedInstallation == null) return;
            Platforms = new ObservableCollection<PlatformModel>(SelectedInstallation.Platforms);
        }
    }
}