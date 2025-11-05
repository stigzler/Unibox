using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Messaging.Enums;

namespace Unibox.Messaging.Messages
{
    public abstract class MessageBase
    {
        public MessageType MessageType { get; set; }
    }
}