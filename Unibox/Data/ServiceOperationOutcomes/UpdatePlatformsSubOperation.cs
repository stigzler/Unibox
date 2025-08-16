using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Enums;
using Unibox.Data.Models;

namespace Unibox.Data.ServiceOperationOutcomes
{
    public class UpdatePlatformsSubOperationOutcome
    {
        public UpdatePlatformMessageType UpdatePlatformMessageType { get; set; }
        public string PlatformName { get; set; } = null;
        public string Summary { get; set; } = String.Empty;
        public DateTime Timestamp { get; set; }

        public UpdatePlatformsSubOperationOutcome(UpdatePlatformMessageType updatePlatformMessageType,
            string summary, string platformName)
        {
            UpdatePlatformMessageType = updatePlatformMessageType;
            Summary = summary;
            PlatformName = platformName;
            Timestamp = DateTime.Now;
        }
    }
}