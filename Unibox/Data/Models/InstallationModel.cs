using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    public class InstallationModel
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();
        public string Name { get; set; } = string.Empty;
        public string InstallationPath { get; set; } = string.Empty;
        public bool OnRemoteMachine { get; set; } = false;
        public DateTime Added { get; set; } = DateTime.UtcNow;
        public ObservableCollection<PlatformModel> Platforms { get; set; } = new ObservableCollection<PlatformModel>();
        public string RomPathRemapLocalRootPath { get; set; } = string.Empty;
        public string RomPathRemapUncRootPath { get; set; } = string.Empty;
    }
}
