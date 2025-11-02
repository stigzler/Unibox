using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;

namespace Unibox.Messages.MessageDetails
{
    internal class EditPlatformMessageDetails
    {
        public PlatformModel Platform { get; set; }
        public InstallationModel Installation { get; set; }
    }
}