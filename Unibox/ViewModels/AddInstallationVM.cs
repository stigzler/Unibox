using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unibox.Data.Models;
using Unibox.Services;

namespace Unibox.ViewModels
{
    internal partial class AddInstallationVM : ObservableObject, IDataErrorInfo
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string installationPath;

        [ObservableProperty]
        private bool onLocalMachine;

        [ObservableProperty]
        private bool isValid;
        public string Error => null;

        public Data.Enums.DialogResult DialogResult { get; set; } = Data.Enums.DialogResult.Unset;

        [RelayCommand]
        private void BrowseForInstallationPath()
        {
            string path = installationsService.GetInstallationsPath();
            if (!string.IsNullOrWhiteSpace(path))
            {
                InstallationPath = path;
            }
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

        private DatabaseService databaseService;
        private InstallationsService installationsService;

        private static readonly string[] ValidatedProperties = { nameof(Name), nameof(InstallationPath) };


        public AddInstallationVM()
        {
        }

        public AddInstallationVM(DatabaseService databaseService, InstallationsService installationsService)
        {
            this.databaseService = databaseService;
            this.installationsService = installationsService;

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
            string error = null;

            switch (propertyName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                        return "Installation name cannot be empty.";
                    if (databaseService.Database.Collections.Installations.FindOne(x => x.Name == Name) != null)
                        return "An Installation with this name already exists.";
                    break;
                case nameof(InstallationPath):
                    OnLocalMachine = !Helpers.FileSystem.IsNetworkPath(InstallationPath);
                    if (string.IsNullOrWhiteSpace(InstallationPath))
                        return "Installation path cannot be empty.";
                    if (databaseService.Database.Collections.Installations.FindOne(x => x.InstallationPath == InstallationPath) != null)
                        return "There is already an Installation for this path.";
                    if (!Helpers.Launchbox.IsLaunchboxRootDirectory(InstallationPath))
                        return "Not a Launchbox root directory.";
                    break;
            }

            return error;
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


    }
}

