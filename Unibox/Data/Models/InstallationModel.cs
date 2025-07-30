using LiteDB;
using System;
using System.Collections.Generic;
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
    }
}
