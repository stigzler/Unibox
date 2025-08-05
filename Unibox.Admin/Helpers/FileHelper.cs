using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Admin.Helpers
{
    internal static class FileHelper
    {
        public static bool IsLaunchboxRootDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            // Check for Launchbox.exe and also, not Launchbox.exe in the "Core" directory
            if (File.Exists(Path.Combine(path, "LaunchBox.exe")) && Directory.Exists(Path.Combine(path, "Core"))) return true;

            return false;
        }
    }
}