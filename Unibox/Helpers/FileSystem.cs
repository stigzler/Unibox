using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static string GetFolderPath(string prompt = "Please select folder", string initialDirectory = "")
        {
            string folderPath = string.Empty;
            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = prompt,
                InitialDirectory = initialDirectory
            };
            if (openFolderDialog.ShowDialog() == true)
            {
                folderPath = openFolderDialog.FolderName;
            }
            return folderPath;
        }

        public static string ReadEmbeddedResourceFile(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(filename));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}