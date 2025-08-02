using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Enums;

namespace Unibox.Data.ServiceOperationOutcomes
{
    internal class UpdatePlatformsOutcome
    {
        public UpdatePlatformOutcome UpdatePlatformOutcome { get; set; } = UpdatePlatformOutcome.Indeterminate;
        public string OutcomeSummary { get; set; } = string.Empty;
    }
}
