using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Admin.Data.Models
{
    internal class SsSystem
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();
        public int ScreenscaperID { get; set; } = 0;
        public string NameEurope { get; set; } = string.Empty;
        public string NameLaunchbox { get; set; } = string.Empty;
        public List<string> OtherNames { get; set; } = new List<string>();
    }
}