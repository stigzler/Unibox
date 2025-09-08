using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.ServiceOperationOutcomes;
using Unibox.Messages;

namespace Unibox.ViewModels
{
    public partial class UpdatePlatformsResultsVM : ObservableObject
    {
        [ObservableProperty]
        private UpdatePlatformsOutcome updatePlatformsOutcome;

        public UpdatePlatformsResultsVM()
        {
        }

        [RelayCommand]
        private void Close(object window)
        {
            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.Installations
            }));
        }
    }
}