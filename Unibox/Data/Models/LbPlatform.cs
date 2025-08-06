using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Models
{
    public class LbPlatform
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();

        /// <summary>
        /// Note: This are the vanilla LB names scraped from Metadata\LaunchBox.Metadata.db
        /// </summary>
        public string Name { get; set; } = "{Unset}";
    }
}