using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    public class LbSsSystemMap
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();
        public LbPlatform? LbPlatform { get; set; } = null;
        public SsSystem? SsSystem { get; set; } = null;
    }
}
