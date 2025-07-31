using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    class PlatformModel
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();
        public string Name { get; set; } = "{Unset}";
        public string ScrapeAs { get; set; } = "{Unset}";
        public string RomFolder { get; set; } = "{Unset}";
    }
}
