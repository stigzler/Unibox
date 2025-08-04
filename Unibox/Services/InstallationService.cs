using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;
using Unibox.Properties;

namespace Unibox.Services
{
    public class InstallationService
    {
        private DatabaseService databaseService;

        public InstallationService(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public ObservableCollection<InstallationModel> GetAllInstallations()
        {
            return new ObservableCollection<InstallationModel>(databaseService.Database.Collections.Installations.FindAll());
        }

        public InstallationModel AddNew(string installtionPath)
        {
            int count = 1;

            while (databaseService.Database.Collections.Installations.FindAll().Where(i => i.Name == $"New Installation [{count}]").Count() > 0)
            {
                count++;
            }

            string newInstallationName = $"New Installation [{count}]";

            InstallationModel installationModel = new InstallationModel
            {
                Name = newInstallationName,
                InstallationPath = installtionPath,
                Added = DateTime.Now,
            };

            databaseService.Database.Collections.Installations.Insert(installationModel);

            return installationModel;
        }

        public bool Delete(InstallationModel installationModel)
        {
            if (installationModel == null)
            {
                throw new ArgumentNullException(nameof(installationModel), "Installation model cannot be null.");
            }
            var result = databaseService.Database.Collections.Installations.Delete(installationModel.ID);
            return result;
        }

        public void Update(InstallationModel installationModel)
        {
            databaseService.Database.Collections.Installations.Upsert(installationModel);
        }

        /// <summary>
        /// This tests if the given name is unique in the Installation Collection.
        /// </summary>
        /// <param name="ignoredInstalltion"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool IsUniqueName(string name, InstallationModel ignoredInstallation = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Installation name cannot be null or empty.", nameof(name));
            }
            var installations = databaseService.Database.Collections.Installations.FindAll().Where(i => i.Name == name);

            if (ignoredInstallation != null)
            {
                installations = installations.Where(i => i.ID != ignoredInstallation.ID);
            }
            return !installations.Any();
        }

        public bool IsUniqueInstallationPath(string path, InstallationModel ignoredInstallation = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Installation path cannot be null or empty.", nameof(path));
            }
            var installations = databaseService.Database.Collections.Installations.FindAll().Where(i => i.InstallationPath == path);
            if (ignoredInstallation != null)
            {
                installations = installations.Where(i => i.ID != ignoredInstallation.ID);
            }
            return !installations.Any();
        }

        public string GetInstallationPath()
        {
            string installationPath = string.Empty;

            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Select the Launchbox root directory for the installation",
                InitialDirectory = Settings.Default.InstallationInitialDirectory,
            };

            if (openFolderDialog.ShowDialog() == true)
            {
                installationPath = openFolderDialog.FolderName;
                Settings.Default.InstallationInitialDirectory = installationPath;
            }

            return installationPath;
        }

        public bool IsLaunchboxRootDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            // Check for Launchbox.exe and also, not Launchbox.exe in the "Core" directory
            if (File.Exists(Path.Combine(path, "LaunchBox.exe")) && Directory.Exists(Path.Combine(path, "Core"))) return true;

            return false;
        }
    }
}