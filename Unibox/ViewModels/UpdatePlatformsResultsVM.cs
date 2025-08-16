using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.ServiceOperationOutcomes;

namespace Unibox.ViewModels
{
    public partial class UpdatePlatformsResultsVM : ObservableObject
    {
        [ObservableProperty]
        private UpdatePlatformsOutcome updatePlatformsOutcome;

        public UpdatePlatformsResultsVM()
        {
        }
    }
}