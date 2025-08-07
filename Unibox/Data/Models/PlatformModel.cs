using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    public class PlatformModel
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();
        public string Name { get; set; } = "{Unset}";
        public string LaunchboxScrapeAs { get; set; } = "{Unset}";
        public string LaunchboxRomFolder { get; set; } = String.Empty;
        public string ResolvedRomFolder { get; set; } = String.Empty;
        public ObservableCollection<PlatformFolderModel> PlatformFolders { get; set; } = new ObservableCollection<PlatformFolderModel>();

        public bool Locked { get; set; } = false;

        public override string ToString()
        {
            return $"{Name} ({LaunchboxScrapeAs}): [{LaunchboxRomFolder}] | [{ResolvedRomFolder}]";
        }
    }
}