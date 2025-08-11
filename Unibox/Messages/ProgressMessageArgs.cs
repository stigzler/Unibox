using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Messages
{
    internal class ProgressMessageArgs
    {
        public string PrimaryMessage { get; set; } = string.Empty;
        public string SecondaryMessage { get; set; } = string.Empty;
        public int PercentageComplete { get; set; } = -1;
        public bool ProgressBarIndeterminate { get; set; } = false;
    }
}