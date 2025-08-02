using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.MessageDetails;

namespace Unibox.Messages
{
     class UpdatePlatformMessage: ValueChangedMessage<UpdatePlatformMessageDetails>
    {
        public UpdatePlatformMessage(UpdatePlatformMessageDetails value): base(value)
        {
                
        }

    }
}
