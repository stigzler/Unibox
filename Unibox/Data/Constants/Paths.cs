using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Constants
{
    internal class Paths
    {
        // Direcotries
        public const string LaunchboxRelDataDir = @"Data";

        public const string LaunchboxRelGamesDir = @"Games";
        public const string LaunchboxRelGamesXmlDir = @"Data\Platforms";
        public const string LaunchboxUniboxPluginDll = @"Plugins\Unibox\Unibox.Plugin.dll";

        public static string LocalAppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unibox");

        // Files
        public const string LaunchboxPlatformsXmlFile = @"Platforms.xml";

        public const string LaunchboxEmulatorsXmlFile = @"Emulators.xml";

        public const string MediaMapFile = @"LbToSsMediaMap.dat";
        public const string PlatformMapFile = @"LbToSsSystemsMap.dat";
        public const string SsMediaTypesFile = @"SsMediaTypesList.dat";
        public const string SsSystemsFile = @"SsSystemList.dat";
        public const string SsRegionsFile = @"SsRegionsList.dat";
        public const string LbPlatformsFile = @"LbPlatfomList.dat";
        public const string LbMediaTypesFile = @"LbMediaTypesList.dat";

        public const string UniboxRelApplicationPluginDllPath = @"Plugin\Unibox.Plugin.dll";
    }
}