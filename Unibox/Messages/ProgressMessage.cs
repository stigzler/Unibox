using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Messages
{
    internal class ProgressMessage : ValueChangedMessage<ProgressMessageArgs>
    {
        public ProgressMessage(ProgressMessageArgs value) : base(value)
        {
        }

        public ProgressMessage(string primaryMessage, string secondaryMessage, int percentageComplete)
            : base(new ProgressMessageArgs
            {
                PrimaryMessage = primaryMessage,
                SecondaryMessage = secondaryMessage,
                PercentageComplete = percentageComplete
            })
        {
        }
    }
}