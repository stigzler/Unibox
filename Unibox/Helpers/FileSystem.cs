using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unibox.Properties;

namespace Unibox.Helpers
{
    internal class FileSystem
    {

        public static bool IsVolumedAndRooted(string path)
        {
            return Path.IsPathRooted(path)
                 && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }

        public static bool IsNetworkPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
 
            if (!path.StartsWith(@"/") && !path.StartsWith(@"\"))
            {
                string rootPath = System.IO.Path.GetPathRoot(path); // get drive's letter
                System.IO.DriveInfo driveInfo = new System.IO.DriveInfo(rootPath); // get info about the drive
                return driveInfo.DriveType == DriveType.Network; // return true if a network drive
            }
            return true; // is a UNC path
        }
        
        public static bool IsValidFilepath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // Combine invalid path and file name characters
            var invalidChars = new HashSet<char>(
                Path.GetInvalidPathChars().Concat(Path.GetInvalidFileNameChars())
            );

            return !path.Any(c => invalidChars.Contains(c));
        }

        public static string GetInstallationsPath()
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

        public static string GetRemapToPath()
        {
            string remapToPath = string.Empty;

            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Select the directory to remap the Launchbox database paths to",
                InitialDirectory = Settings.Default.RemapToInitialDirectory            

            };

            if (openFolderDialog.ShowDialog() == true)
            {
                remapToPath = openFolderDialog.FolderName;
                Settings.Default.RemapToInitialDirectory = remapToPath;
            }

            return remapToPath;
        }

        public static bool IsLaunchboxRootDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            // Check for Launchbox.exe and also, not Launchbox.exe in the "Core" directory
            if (File.Exists(Path.Combine(path, "LaunchBox.exe")) && Directory.Exists(Path.Combine(path, "Core"))) return true;

            return false;
        }
    }
}
