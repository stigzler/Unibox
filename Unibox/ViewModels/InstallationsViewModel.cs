using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;
using Unibox.Services;

namespace Unibox.ViewModels
{
   public partial class InstallationsViewModel: ObservableObject
    {
        [ObservableProperty]
        private IEnumerable<InstallationModel> installations;

        public DatabaseService DatabaseService;

        public InstallationsViewModel()
        {
            
        }

        public InstallationsViewModel(DatabaseService databaseService)
        {
            DatabaseService = databaseService;

            Installations = DatabaseService.Database.Collections.Installations.FindAll();

        }

    }
}
