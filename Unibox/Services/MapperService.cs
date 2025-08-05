using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Services
{
    internal class MapperService
    {
        public Dictionary<string, string> LbToSsMediaMaps { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> LbToSsIdPlatformMaps { get; set; } = new Dictionary<string, string>();

        public MapperService()
        {
            PopulateMediaMapDict();
            PopulatePlatformMapDict();
        }

        private void PopulateMediaMapDict()
        {
            string[] mediaFileLines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory,
                Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.MediaMapFile));
            foreach (string line in mediaFileLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                string[] parts = line.Split(new[] { '|' }, 2);
                if (parts.Length == 2)
                {
                    string lbMediaType = parts[0].Trim();
                    string ssMediaType = parts[1].Trim();
                    if (!LbToSsMediaMaps.ContainsKey(lbMediaType))
                    {
                        LbToSsMediaMaps.Add(lbMediaType, ssMediaType);
                    }
                }
            }
        }

        private void PopulatePlatformMapDict()
        {
            string[] platformFileLines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory,
                Data.Constants.Paths.LaunchboxRelDataDir, Data.Constants.Paths.PlatformMapFile));
            foreach (string line in platformFileLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                string[] parts = line.Split(new[] { '|' }, 2);
                if (parts.Length == 2)
                {
                    string lbPlatformId = parts[0].Trim();
                    string ssPlatformId = parts[1].Trim();
                    if (!LbToSsIdPlatformMaps.ContainsKey(lbPlatformId))
                    {
                        LbToSsIdPlatformMaps.Add(lbPlatformId, ssPlatformId);
                    }
                }
            }
        }
    }
}