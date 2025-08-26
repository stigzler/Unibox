using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Enums;

namespace Unibox.Messages.MessageDetails
{
    internal class UpdatePlatformMessageDetails
    {
        public UpdatePlatformMessageType MessageType { get; set; }
        public string SummaryLine { get; set; } = string.Empty;
    }
}