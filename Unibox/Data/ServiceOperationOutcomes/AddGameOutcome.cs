using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;

namespace Unibox.Data.ServiceOperationOutcomes
{
    internal class AddGameOutcome
    {
        public GameModel Game { get; set; } = null;
        public List<string> Outcomes { get; set; } = new List<string>();
    }
}