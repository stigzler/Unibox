using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;

namespace Unibox.Messages.MessageDetails
{
    internal class EditGameMessageDetails
    {
        public GameModel Game { get; set; }
        public InstallationModel Installation { get; set; }
    }
}