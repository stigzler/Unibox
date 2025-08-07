using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    public class PlatformFolderModel
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();

        public LbMediaType? MediaType { get; set; } = null;

        public string LaunchboxMediaPath { get; set; } = String.Empty;

        public string ResolvedMediaPath { get; set; } = String.Empty;

        public bool Locked { get; set; } = false;
    }
}