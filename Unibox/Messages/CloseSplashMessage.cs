using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Messages
{
    internal class CloseSplashMessage : ValueChangedMessage<bool>
    {
        public CloseSplashMessage(bool value) : base(value)
        {
        }
    }
}