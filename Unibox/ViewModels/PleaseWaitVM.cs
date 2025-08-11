using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Messages;

namespace Unibox.ViewModels
{
    internal partial class PleaseWaitVM : ObservableObject, IRecipient<ProgressMessage>
    {
        [ObservableProperty]
        private string text = "Please wait whilst operation finishes...Please wait whilst operation finishes...Please wait whilst operation finishes...Please wait whilst operation finishes...Please wait whilst operation finishes...Please wait whilst operation finishes...";

        [ObservableProperty]
        private string primaryText = "Running Operation...";

        [ObservableProperty]
        private string secondaryText = "Please wait...";

        public PleaseWaitVM()
        {
            WeakReferenceMessenger.Default.Register<ProgressMessage>(this);
        }

        public void Receive(ProgressMessage message)
        {
            Debug.WriteLine(message);
            if (!String.IsNullOrWhiteSpace(message.Value.PrimaryMessage)) PrimaryText = message.Value.PrimaryMessage;
            if (!String.IsNullOrWhiteSpace(message.Value.SecondaryMessage)) SecondaryText = message.Value.SecondaryMessage;
        }
    }
}