using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Enums;

namespace Unibox.Messages
{
    internal class PageChangeMessage : ValueChangedMessage<PageChangeMessageArgs>
    {
        public PageChangeMessage(PageChangeMessageArgs value) : base(value)
        {
        }

        public PageChangeMessage(PageRequestType requestType)
          : base(new PageChangeMessageArgs
          {
              RequestType = requestType,
          })
        {
        }

        public PageChangeMessage(PageRequestType requestType, object data)
          : base(new PageChangeMessageArgs
          {
              RequestType = requestType,
              Data = data
          })
        {
        }
    }
}