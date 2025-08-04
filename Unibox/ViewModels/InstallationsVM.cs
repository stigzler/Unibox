using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Services;
using Unibox.Views;

namespace Unibox.ViewModels
{
    internal partial class InstallationsVM : ObservableObject, IRecipient<InstallationUpdatedMessage>
    {
        [ObservableProperty]
        private ObservableCollection<InstallationModel> installations = new ObservableCollection<InstallationModel>();

        [ObservableProperty]
        private ObservableCollection<PlatformModel> platforms = new ObservableCollection<PlatformModel>();

        [ObservableProperty]
        private InstallationModel selectedInstallation;

        [ObservableProperty]
        private PlatformModel selectedPlatform;

        private DatabaseService databaseService;
        private InstallationService installationService;
        private PlatformService platformService;

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

        partial void OnSelectedInstallationChanged(InstallationModel value)
        {
            UpdatePlatformsList();
        }

        private void UpdatePlatformsList()
        {
            if (SelectedInstallation == null) return;
            Platforms = new ObservableCollection<PlatformModel>(SelectedInstallation.Platforms);
        }

        private void UpdateIstallationsFromDatabase()
        {
            Installations = installationService.GetAllInstallations();
        }

        [RelayCommand]
        private void EditInstallation()
        {
            if (selectedInstallation == null) return;

            EditInstallationForm editInstallationForm = new EditInstallationForm();
            editInstallationForm.ViewModel.Installation = SelectedInstallation;
            editInstallationForm.ShowDialog();
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
        private void UpdatePlatforms()
        {
            if (selectedInstallation == null) return;

            platformService.UpdateInstallationPlatforms(selectedInstallation);

            UpdatePlatformsList();
        }

        void IRecipient<InstallationUpdatedMessage>.Receive(InstallationUpdatedMessage message)
        {
            UpdateIstallationsFromDatabase();
        }
    }
}