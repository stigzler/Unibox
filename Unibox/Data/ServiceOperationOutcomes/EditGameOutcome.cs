using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;

namespace Unibox.Data.ServiceOperationOutcomes
{
    public class EditGameOutcome
    {
        public bool GameEdited { get; set; } = false;
        public string Outcome { get; set; } = string.Empty;
    }
}