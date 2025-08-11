using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.ServiceOperationOutcomes;

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
    }
}