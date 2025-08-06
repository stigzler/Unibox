using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    public class SsSystem
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();
        public int SsID { get; set; } = 0;

        /// <summary>
        /// Note: this is the Screenscraper Europe name
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}