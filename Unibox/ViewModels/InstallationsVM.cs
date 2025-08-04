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
    public partial class InstallationsVM : ObservableObject, IRecipient<InstallationUpdatedMessage>
    {
        [ObservableProperty]
        private ObservableCollection<InstallationModel> installations = new ObservableCollection<InstallationModel>();

        [ObservableProperty]
        private InstallationModel selectedInstallation;

        private DatabaseService databaseService;
        private InstallationService installationService;

        public InstallationsVM()
        {
        }

        public InstallationsVM(DatabaseService databaseService, InstallationService installationService)
        {
            this.databaseService = databaseService;
            this.installationService = installationService;

            WeakReferenceMessenger.Default.Register<InstallationUpdatedMessage>(this);

            UpdateIstallationsFromDatabase();
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
                    $"Please edit this to set it up. Name: {newInstallation.Name}", "New Installaiton created",
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

        void IRecipient<InstallationUpdatedMessage>.Receive(InstallationUpdatedMessage message)
        {
            UpdateIstallationsFromDatabase();
        }
    }
}