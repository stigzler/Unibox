using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Unibox.Data.Models;
using Unibox.Data.ServiceOperationOutcomes;
using Unibox.Messages;
using Unibox.Services;

namespace Unibox.ViewModels
{
    public partial class EditPlatformVM : ObservableObject
    {
        [ObservableProperty]
        private string headerText;

        [ObservableProperty]
        private PlatformModel platform;

        [ObservableProperty]
        private InstallationModel installation;

        private InstallationService installationService;

        public EditPlatformVM(InstallationService installationService)
        {
            this.installationService = installationService;
        }

        public EditPlatformVM()
        {
        }

        partial void OnPlatformChanged(PlatformModel value)
        {
            if (value != null)
            {
                HeaderText = $"Edit Platform: {value.Name}";
            }
        }

        [RelayCommand]
        private void NavigateToInstallations()
        {
            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.Installations
            }));
        }

        [RelayCommand]
        private void SavePlatform()
        {
            installationService.Update(Installation);
            NavigateToInstallations();
            WeakReferenceMessenger.Default.Send(new InstallationChangedMessage(Installation));
        }

        [RelayCommand]
        private void GetAppPath()
        {
            var ofd = new OpenFileDialog()
            {
                Title = "Please select an application to run for this platform."
            };
            if (Directory.Exists(Platform.ApplicationPath))
            {
                ofd.InitialDirectory = Platform.ApplicationPath;
            }
            else
            {
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }

            if (ofd.ShowDialog() == true)
            {
                Platform.ApplicationPath = ofd.FileName;
                OnPropertyChanged(nameof(Platform));
                WeakReferenceMessenger.Default.Send(new InstallationChangedMessage(Installation));
            }
        }

        [RelayCommand]
        private void ClearAppPath()
        {
            Platform.ApplicationPath = string.Empty;
            OnPropertyChanged(nameof(Platform));
            WeakReferenceMessenger.Default.Send(new InstallationChangedMessage(Installation));
        }
    }
}