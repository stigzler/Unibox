using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Plugin.Messages
{
    internal class SystemMenuMessage : ValueChangedMessage<bool>
    {
        public SystemMenuMessage(bool value) : base(value)
        {
        }
    }
}