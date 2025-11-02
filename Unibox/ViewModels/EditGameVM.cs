using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Services;

namespace Unibox.ViewModels
{
    internal partial class EditGameVM : ObservableObject
    {
        [ObservableProperty]
        private GameModel game;

        [ObservableProperty]
        private InstallationModel installation;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NavigateToGamesCommand))]
        private bool notSaving = true;

        [ObservableProperty]
        private string alertText;

        [ObservableProperty]
        private SolidColorBrush alertColor;

        [ObservableProperty]
        private bool alertVisible;

        [ObservableProperty]
        private ObservableCollection<string> mediaTypes;

        [ObservableProperty]
        private string selectedMediaType;

        [ObservableProperty]
        private ImageSource imageSource = new BitmapImage(
            new Uri("pack://application:,,,/Unibox;component/Resources/Images/noMedia.png"));

        private GameService gameService;

        public EditGameVM(GameService gameService)
        {
            this.gameService = gameService;
            mediaTypes = new ObservableCollection<string>(Unbroken.LaunchBox.Plugins.Data.ImageTypes.GetList());
            mediaTypes.Add("Video");
            mediaTypes.Add("Manual");
            mediaTypes.Add("Music");
            mediaTypes = new ObservableCollection<string>(mediaTypes.OrderBy(mt => mt));
        }

        public EditGameVM()
        {
        }

        [RelayCommand(CanExecute = nameof(NotSaving))]
        private void NavigateToGames()
        {
            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.Games
            }));
        }

        [RelayCommand]
        private async Task SaveGameEdit()
        {
            NotSaving = false;

            var addGameOutcome = await gameService.EditGame(Game, Installation);

            if (addGameOutcome.GameEdited)
            {
                AlertText = $"Game Edit Successful";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 128, 0));
            }
            else
            {
                AlertText = $"Game Edit Unsuccessful:\r\n{addGameOutcome.Outcome}";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 0, 0));
            }

            NotSaving = true;

            AlertVisible = true;
            await Task.Delay(2000);
            AlertVisible = false;
        }

        [RelayCommand]
        private void CheckForMedia(string mediaType)
        {
            Debug.WriteLine($"Checking for media of type: {mediaType}");
            List<string> mediaList = gameService.GetGameMediaPaths(Game, mediaType);

            switch (SelectedMediaType)
            {
                case "Video":
                    mediaList = mediaList.Where(m => Path.GetExtension(m).ToLower() is ".mp4" or ".avi" or ".mov" or ".wmv").ToList();
                    break;
            }
        }
    }
}