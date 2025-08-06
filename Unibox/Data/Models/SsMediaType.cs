using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    public class SsMediaType
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();

        /// <summary>
        /// Note: This matches the Enum Name in ScreenscraperWrapper, NOT the MediaTypeName in Screenscraper
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}