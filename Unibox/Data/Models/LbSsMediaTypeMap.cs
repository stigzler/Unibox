using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    public class LbSsMediaTypeMap
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();
        public LbMediaType? LbMediaType { get; set; } = null;
        public List<SsMediaType>? SsMediaType { get; set; } = new List<SsMediaType>();
    }
}