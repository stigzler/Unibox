using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;

namespace Unibox.Helpers
{
    internal class Plugin
    {
        internal static string GetApplicationPluginVersion()
        {
            string fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Data.Constants.Paths.UniboxRelApplicationPluginDllPath);
            var versionInfo = FileVersionInfo.GetVersionInfo(fullpath);
            string version = versionInfo.FileVersion;
            return version;
        }

        internal static string GetLaunchboxPluginVersion(InstallationModel installationModel)
        {
            string fullpath = Path.Combine(installationModel.InstallationPath, Data.Constants.Paths.LaunchboxUniboxPluginDll);
            var versionInfo = FileVersionInfo.GetVersionInfo(fullpath);
            string version = versionInfo.FileVersion;
            return version;
        }

        internal static bool IsUniboxPluginInstalled(InstallationModel installationModel)
        {
            string fullpath = Path.Combine(installationModel.InstallationPath, Data.Constants.Paths.LaunchboxUniboxPluginDll);
            return File.Exists(fullpath);
        }

        internal static bool PluginRequiresUpdating(InstallationModel installationModel)
        {
            Version applicationVersion = new Version(GetApplicationPluginVersion());
            Version launchboxVersion = new Version(GetLaunchboxPluginVersion(installationModel));

            var result = applicationVersion.CompareTo(launchboxVersion);
            if (result > 0)
            {
                Debug.WriteLine($"Unibox Plugin version {applicationVersion} is newer than the installed version {launchboxVersion} in {installationModel.Name}. Please update the plugin.");
                return true;
            }
            else if (result <= 0)
            {
                Debug.WriteLine($"Unibox Plugin version {applicationVersion} is older than the installed version {launchboxVersion} in {installationModel.Name}. Please update Unibox.");
                return false;
            }
            return false;
        }

        internal static Exception UpdatePlugin(InstallationModel installationModel)
        {
            string appDir = Path.GetDirectoryName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                Data.Constants.Paths.UniboxRelApplicationPluginDllPath));

            string pluginDir = Path.GetDirectoryName(Path.Combine(installationModel.InstallationPath,
                Data.Constants.Paths.LaunchboxUniboxPluginDll));

            if (!Directory.Exists(pluginDir))
            {
                Directory.CreateDirectory(pluginDir);
            }

            try
            {
                FileSystem.CopyFilesRecursively(appDir, pluginDir);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}