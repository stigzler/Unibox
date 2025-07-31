using LiteDB;
using System;
using System.Collections.Generic;
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
        public bool OnLocalMachine { get; set; } = true;
        public DateTime Added { get; set; } = DateTime.UtcNow;
    }
}
