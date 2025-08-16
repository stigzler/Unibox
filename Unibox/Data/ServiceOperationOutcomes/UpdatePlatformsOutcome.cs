using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Enums;

namespace Unibox.Data.ServiceOperationOutcomes
{
    public class UpdatePlatformsOutcome
    {
        public UpdatePlatformOutcome OverallOutcome { get; set; } = UpdatePlatformOutcome.Indeterminate;
        public string OutcomeSummary { get; set; } = string.Empty;
        public List<UpdatePlatformsSubOperationOutcome> SubOperationOutcomes { get; set; } = new List<UpdatePlatformsSubOperationOutcome>();
    }
}