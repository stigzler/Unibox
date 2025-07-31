using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Services
{
    public  class InstallationsService
    {
        public  string GetInstallationsPath()
        {
            string installationPath = string.Empty;

            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Select the Launchbox root directory on the network",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts)
            };

            if (openFolderDialog.ShowDialog() == true)
            {
                installationPath = openFolderDialog.FolderName;
            }
            else
            {
                // Handle the case where the user cancels the dialog
                throw new OperationCanceledException("User canceled the folder selection.");
            }

            return installationPath;
        }

    }
}
