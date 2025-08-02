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

        public string MediaType { get; set; } = String.Empty;

        public string Folderpath { get; set; } = String.Empty;
    }
}
