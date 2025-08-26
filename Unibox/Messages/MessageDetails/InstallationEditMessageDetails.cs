using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;

namespace Unibox.Messages.MessageDetails
{
    internal class InstallationEditMessageDetails
    {
        public InstallationModel Installation { get; set; }
        public string RomsReplaceText { get; set; } = string.Empty;
        public string MediaReplaceText { get; set; } = string.Empty;

        public InstallationEditMessageDetails()
        {
        }

        public InstallationEditMessageDetails(InstallationModel installation)
        {
            Installation = installation;
        }

        public InstallationEditMessageDetails(InstallationModel installation, string romsReplaceText, string mediaReplaceText)
        {
            Installation = installation;
            RomsReplaceText = romsReplaceText;
            MediaReplaceText = mediaReplaceText;
        }
    }
}