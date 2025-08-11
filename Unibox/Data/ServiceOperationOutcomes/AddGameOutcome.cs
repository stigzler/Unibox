using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;

namespace Unibox.Data.ServiceOperationOutcomes
{
    public class AddGameOutcome
    {
        public string RomPath { get; set; } = null;
        public GameModel Game { get; set; } = null;
        public List<string> Outcomes { get; set; } = new List<string>();
        public bool RomAdded { get; set; } = false;
    }
}