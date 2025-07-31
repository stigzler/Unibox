using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;
using Unibox.Services;
using Unibox.Views;

namespace Unibox.ViewModels
{
   public partial class InstallationsVM: ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<InstallationModel> installations = new ObservableCollection<InstallationModel>();

        private DatabaseService databaseService;
        private InstallationsService installationsService;

        public InstallationsVM()
        {             
        }

        public InstallationsVM(DatabaseService databaseService, InstallationsService installationsService)
        {
            this.databaseService = databaseService;
            this.installationsService = installationsService;

            UpdateIstallationsFromDatabase();
        }

        private void UpdateIstallationsFromDatabase()
        {
            Installations = new ObservableCollection<InstallationModel>(databaseService.Database.Collections.Installations.FindAll());
        }

        [RelayCommand]
        private void AddNewInstallation()
        {
            //string path = installationsService.GetInstallationsPath();

            //return;
            //InstallationModel newInstallation = new InstallationModel()
            //{
            //    Name = $"New Installation {Installations.Count() +1}",
            //    InstallationPath = "//Atari1280/C:/Launchbox"
            //};

            AddInstallationForm addInstallationForm = new AddInstallationForm();
            addInstallationForm.ShowDialog();

            //WeakReferenceMessenger.Default.Send(new Messages.InstallationAddedMessage(newInstallation));

            //UpdateIstallationsFromDatabase();

            //databaseService.Database.Collections.Installations.Insert(newInstallation);
        }

    }
}
