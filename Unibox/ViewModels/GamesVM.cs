using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Unibox.Data.Models;
using Unibox.Services;

namespace Unibox.ViewModels
{
    internal partial class GamesVM : ObservableObject
    {
        private DatabaseService databaseService;

        private ObservableCollection<InstallationModel> installations;

        [ObservableProperty]
        private CollectionView installationsView;

        private ObservableCollection<PlatformModel> platforms;

        [ObservableProperty]
        private CollectionView platformsView;

        [ObservableProperty]
        private InstallationModel selectedInstallation;

        [ObservableProperty]
        private PlatformModel selectedPlatform;

        [ObservableProperty]
        private bool installationAvailable = false;

        [ObservableProperty]
        private Cursor cursor = Cursors.Arrow;

        public GamesVM(DatabaseService databaseService)
        {
            this.databaseService = databaseService;

            installations = new ObservableCollection<InstallationModel>(databaseService.Database.Collections.Installations.FindAll());

            InstallationsView = (CollectionView)CollectionViewSource.GetDefaultView(installations);
        }

        public GamesVM()
        {
        }

        partial void OnSelectedInstallationChanged(InstallationModel value)
        {
            Cursor = Cursors.Wait;
            InstallationAvailable = Directory.Exists(value.InstallationPath);
            Cursor = Cursors.Arrow;

            platforms = value.Platforms;
            PlatformsView = (CollectionView)CollectionViewSource.GetDefaultView(platforms);
            PlatformsView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
        }
    }
}