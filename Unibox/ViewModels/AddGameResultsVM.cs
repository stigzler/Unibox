using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.ServiceOperationOutcomes;
using Unibox.Messages;
using Unibox.Messages.MessageDetails;

namespace Unibox.ViewModels
{
    public partial class AddGameResultsVM : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<AddGameOutcome> addGameResults = new ObservableCollection<AddGameOutcome>();

        [ObservableProperty]
        private AddGameOutcome selectedOutcome;

        public AddGameResultsVM()
        {
        }

        [RelayCommand]
        private void CloseResults()
        {
            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.Games,
                Data = new GamesPageMessageDetails() { GamesUpdateRequired = true }
            }));
        }
    }
}