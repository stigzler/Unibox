using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    public class GameModel
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();
        public string Title { get; set; } = string.Empty;
        public PlatformModel Platform { get; set; } = null;
        public string ApplicationPath { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; } = DateTime.MinValue;
        public string Notes { get; set; } = string.Empty;
    }
}