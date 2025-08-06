using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unibox.Messages;
using Unibox.Services;

namespace Unibox.ViewModels
{
    internal partial class SplashVM : ObservableObject, IRecipient<ProgressMessage>
    {
        [ObservableProperty]
        private string primaryText = "Welcome to Unibox";

        [ObservableProperty]
        private string secondaryText = "Spinning up the wassiknackers and oojamma-wotsits...";

        [ObservableProperty]
        private double mainProgressBarValue = 100;

        // private DatabaseService databaseService;

        //  public SplashVM(DatabaseService databaseService)
        public SplashVM()

        {
            WeakReferenceMessenger.Default.Register<ProgressMessage>(this);
            WeakReferenceMessenger.Default.Send(new CloseSplashMessage(true));
        }

        public void Receive(ProgressMessage message)
        {
            PrimaryText = message.Value.PrimaryMessage;
            SecondaryText = message.Value.SecondaryMessage;
            MainProgressBarValue = message.Value.PercentageComplete;
        }
    }
}