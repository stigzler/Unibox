using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Serialization;
using Unibox.Data.Models;
using Unibox.Messages;
using Unibox.Messages.MessageDetails;
using Unibox.Properties;
using Unibox.Services;

namespace Unibox.ViewModels
{
    internal partial class EditGameVM : ObservableObject
    {
        [ObservableProperty]
        private bool dialogTextUnfocussed = true;

        [ObservableProperty]
        private SolidColorBrush alertColor;

        [ObservableProperty]
        private string alertText;

        [ObservableProperty]
        private bool alertVisible;

        [ObservableProperty]
        private bool dialogButtonsVisible = false;

        [ObservableProperty]
        private GameModel game;

        private GameService gameService;

        [ObservableProperty]
        private ImageSource imageSource = new BitmapImage(
            new Uri("pack://application:,,,/Unibox;component/Resources/Images/noMedia.png"));

        [ObservableProperty]
        private InstallationModel installation;

        [ObservableProperty]
        private Dictionary<string, bool> mediaAvailable = new Dictionary<string, bool>()
              {
                { "Box - 3D", false },
                { "Box - Front", false},
                { "Box - Back", false},
                { "Music", false},
                { "Manual", false},
                { "Video", false},
                { "Screenshot - Gameplay", false},
                { "Screenshot - Game Title", false},
                { "Clear Logo", false},
                { "Cart - Front", false},
                { "Disc", false},
                { "Fanart - Background", false},
            };

        private Dictionary<string, List<string>> mediaExtensionsDCT = new Dictionary<string, List<string>>();

        [ObservableProperty]
        private string mediaItemCounter = "0 / 0";

        [ObservableProperty]
        private ObservableCollection<string> mediaList = new ObservableCollection<string>();

        private int mediaListIndex = 0;

        [ObservableProperty]
        private ObservableCollection<string> mediaTypes;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NavigateToGamesCommand))]
        private bool notSaving = true;

        [ObservableProperty]
        private string pageTitle = "Edit Game";

        [ObservableProperty]
        private string selectedMediaItem = "{None}";

        [ObservableProperty]
        private string selectedMediaType;

        public EditGameVM(GameService gameService)
        {
            this.gameService = gameService;
            mediaTypes = new ObservableCollection<string>(Unbroken.LaunchBox.Plugins.Data.ImageTypes.GetList());
            mediaTypes.Add("Video");
            mediaTypes.Add("Manual");
            mediaTypes.Add("Music"); // I'm not sure LB even supports Music in PlatformFolders Collection?? If established not, then remove this
            mediaTypes = new ObservableCollection<string>(mediaTypes.OrderBy(mt => mt));
            SetMediaExtensions();

            Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        public EditGameVM()
        {
        }

        [RelayCommand]
        private void CheckForMedia(string mediaType)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            List<string> candidateMedia = GetFilteredMediaList(mediaType);

            if (candidateMedia == null || candidateMedia.Count == 0)
            {
                SelectedMediaItem = "No Media";
                MediaItemCounter = "0/0";
                mediaListIndex = 0;
                ImageSource = new BitmapImage(
                    new Uri("pack://application:,,,/Unibox;component/Resources/Images/noMedia.png"));
                MediaList.Clear();
                return;
            }

            //string resolvedMediaType = "Image";
            //if (mediaType == "Video" || mediaType == "Music" || mediaType == "Manual")
            //{
            //    resolvedMediaType = mediaType;
            //}

            //candidateMedia = candidateMedia.Where(m => mediaExtensionsDCT[resolvedMediaType]
            //        .Contains(Path.GetExtension(m).ToLower().Replace(".", ""))).ToList();

            MediaList = new ObservableCollection<string>(candidateMedia);
            mediaListIndex = 0;
            UpdateMediaItem(mediaType);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Default_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // picks up on permitable media extension changes in settings
            //Debug.WriteLine("here");
            if (e.PropertyName == "AppPermittedVideoExts" || e.PropertyName == "AppPermittedMusicExts" ||
                e.PropertyName == "AppPermittedManualExts" || e.PropertyName == "AppPermittedImageExts")
            {
                SetMediaExtensions();
            }
        }

        [RelayCommand]
        private async void DeleteGame(string command = "Indeterminate")
        {
            // Below logic attached to View system where Dialog Buttons panel overlays the normal Nav
            // buttons with the Delete button on. First press shows the dialog buttons, second press confirms deletion.
            if (!DialogButtonsVisible)
            {
                AlertText = $"Delete Game?";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 100, 0));
                DialogButtonsVisible = true;
                return;
            }
            //DialogButtonsVisible = false;

            if (command == "Cancel")
            {
                DialogButtonsVisible = false;
                return;
            }

            var deleteGameOutcome = await gameService.DeleteGame(Game, Installation);

            DialogTextUnfocussed = false;

            if (deleteGameOutcome.GameDeleted)
            {
                AlertText = $"Game Deletion successful";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 128, 0));
                await ShowAlertAsync();
            }
            else
            {
                AlertText = $"Delete Errors: {deleteGameOutcome.Outcome}";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 0, 0));
                await ShowAlertAsync(5000);
            }

            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.Games,
                Data = new GamesPageMessageDetails() { GamesUpdateRequired = true }
            }));
        }

        private List<string> GetFilteredMediaList(string mediaType)
        {
            List<string> candidateMedia = gameService.GetGameMediaPaths(Game, mediaType);
            if (candidateMedia == null || candidateMedia.Count == 0)
            {
                return new List<string>();
            }
            string resolvedMediaType = "Image";
            if (mediaType == "Video" || mediaType == "Music" || mediaType == "Manual")
            {
                resolvedMediaType = mediaType;
            }
            candidateMedia = candidateMedia.Where(m => mediaExtensionsDCT[resolvedMediaType]
                    .Contains(Path.GetExtension(m).ToLower().Replace(".", ""))).ToList();
            return candidateMedia;
        }

        [RelayCommand]
        private void LoadMedia(string mediaType)
        {
            //CheckForMedia(mediaType);
            SelectedMediaType = mediaType;
        }

        [RelayCommand(CanExecute = nameof(NotSaving))]
        private void NavigateToGames()
        {
            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.Games
            }));
        }

        partial void OnGameChanged(GameModel? oldValue, GameModel newValue)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            PageTitle = $"Edit Game ({newValue.Platform.Name})";
            SelectedMediaType = "Clear Logo";
            AlertVisible = false;
            DialogButtonsVisible = false;
            DialogTextUnfocussed = true;
            // SetMediaExtensions(); // this done here in case user changes extensions in settings.
            UpdateMediaAvailability();
            CheckForMedia("Clear Logo");
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        partial void OnSelectedMediaTypeChanged(string value)
        {
            CheckForMedia(value);
        }

        [RelayCommand]
        private void OpenMedia()
        {
            if (SelectedMediaItem == "No Media") return;

            try
            {
                // Use the file's path directly. Windows uses the extension to find the default program.
                // On .NET Core / .NET 5+ you must set UseShellExecute = true to open the file
                var startInfo = new ProcessStartInfo(SelectedMediaItem)
                {
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                AlertText = $"Could not open media item. Exception: {ex.Message}";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 0, 0));
                ShowAlertAsync(5000);
                return;
            }
        }

        [RelayCommand]
        private void OpenMediaDirectory()
        {
            if (MediaList.Count == 0) return;
            string mediaPath = Path.GetDirectoryName(SelectedMediaItem);
            if (Directory.Exists(mediaPath))
            {
                Process.Start("explorer.exe", mediaPath);
            }
        }

        [RelayCommand]
        private void ProcessNextMediaItem()
        {
            if (MediaList.Count == 0) return;
            mediaListIndex++;
            if (mediaListIndex >= MediaList.Count)
            {
                mediaListIndex = 0;
            }
            UpdateMediaItem();
        }

        [RelayCommand]
        private async Task SaveGameEdit()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            NotSaving = false;

            var addGameOutcome = await gameService.EditGame(Game, Installation);

            Mouse.OverrideCursor = Cursors.Arrow;

            if (addGameOutcome.GameEdited)
            {
                AlertText = $"Metadata update successful";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 128, 0));
                await ShowAlertAsync();
            }
            else
            {
                AlertText = $"Update Unsuccessful:{addGameOutcome.Outcome}";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 0, 0));
                await ShowAlertAsync(5000);
            }

            NotSaving = true;
        }

        [RelayCommand]
        private async void SearchInternet()
        {
            string searchTerm = $"{Game.Title} ({Game.Platform.Name}) {SelectedMediaType}";

            // 1. URL-encode the search term
            // This converts spaces and special characters into a format the URL can handle (%20, %2B, etc.).
            string encodedSearchTerm = HttpUtility.UrlEncode(searchTerm);

            // 2. Construct the full search URL (using Google as an example)
            string searchUrl = $"https://www.google.com/search?q={encodedSearchTerm}";

            // 3. Open the default browser to the constructed URL
            try
            {
                // Use 'startInfo' to ensure compatibility across different .NET versions and OSes (Windows/Linux/macOS)
                Process.Start(new ProcessStartInfo(searchUrl) { UseShellExecute = true });
                //throw new Exception();
            }
            catch (Exception ex)
            {
                AlertText = $"Could not open internet browser via Process.Start. Have you set a default app for http enquiries?";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 0, 0));

                await ShowAlertAsync(5000);
            }
        }

        private void SetMediaExtensions()
        {
            mediaExtensionsDCT = new Dictionary<string, List<string>>()
            {
                { "Video", Settings.Default.AppPermittedVideoExts.Split(',').ToList<string>() },
                { "Music", Settings.Default.AppPermittedMusicExts.Split(',').ToList<string>() },
                { "Manual", Settings.Default.AppPermittedManualExts.Split(',').ToList<string>()},
                { "Image", Settings.Default.AppPermittedImageExts.Split(',').ToList<string>()}
            };
        }

        private async Task ShowAlertAsync(int showFor = 2000)
        {
            AlertVisible = true;
            await Task.Delay(showFor);
            AlertVisible = false;
        }

        private void UpdateMediaAvailability()
        {
            foreach (string mediaType in MediaAvailable.Keys.ToList())
            {
                List<string> candidateMedia = GetFilteredMediaList(mediaType);
                MediaAvailable[mediaType] = candidateMedia.Count > 0;
            }
            OnPropertyChanged("MediaAvailable");
        }

        /// <summary>
        /// This is messy, but remedial as lost will to live
        /// </summary>
        /// <param name="mediaType"></param>
        private void UpdateMediaItem(string mediaType = null)
        {
            SelectedMediaItem = MediaList[mediaListIndex];
            MediaItemCounter = $"{mediaListIndex + 1}/{MediaList.Count}";

            if (mediaType == null) mediaType = SelectedMediaType;

            switch (mediaType)
            {
                case "Video":
                    ImageSource = new BitmapImage(
                        new Uri("pack://application:,,,/Unibox;component/Resources/Images/videoMedia.png"));
                    break;

                case "Music":
                    ImageSource = new BitmapImage(
                        new Uri("pack://application:,,,/Unibox;component/Resources/Images/musicMedia.png"));
                    break;

                case "Manual":
                    ImageSource = new BitmapImage(
                        new Uri("pack://application:,,,/Unibox;component/Resources/Images/manualMedia.png"));
                    break;

                default:
                    try
                    {
                        ImageSource = Helpers.Image.UnlockedImageCopy(SelectedMediaItem);
                    }
                    catch (Exception e)
                    {
                        AlertText = $"Could not open game image. Exception: {e.Message}";
                        AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 0, 0));
                        ShowAlertAsync(5000);
                        ImageSource = new BitmapImage(
                            new Uri("pack://application:,,,/Unibox;component/Resources/Images/noMedia.png"));
                    }
                    break;
            }
        }

        [RelayCommand]
        private async void UpdatePrimaryMediaItem()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = $"Please select the new primary Media Item for: {SelectedMediaType}",
            };

            string filter;
            switch (SelectedMediaType)
            {
                case "Video":
                case "Music":
                case "Manual":
                    openFileDialog.Filter = $"{SelectedMediaType} Files|*." + String.Join(";*.", mediaExtensionsDCT[SelectedMediaType]);
                    break;

                default:
                    openFileDialog.Filter = $"Image Files|*." + String.Join(";*.", mediaExtensionsDCT["Image"]);
                    break;
            }

            openFileDialog.ShowDialog();

            if (string.IsNullOrEmpty(openFileDialog.FileName)) return;

            PlatformFolderModel platformFolder = Game.Platform.PlatformFolders
                                                .Where(pf => pf.MediaType.ToString() == SelectedMediaType).FirstOrDefault();

            if (platformFolder == null)
            {
                AlertText = $"Could not find Platform Folder for Media Type: {SelectedMediaType}";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 0, 0));
                await ShowAlertAsync(5000);
                return;
            }

            string prospectiveFilepath = Path.Combine(platformFolder.ResolvedMediaPath,
               $"{Path.GetFileNameWithoutExtension(Game.ApplicationPath)}-00{Path.GetExtension(openFileDialog.FileName)}");

            try
            {
                File.Copy(openFileDialog.FileName, prospectiveFilepath, true);
                AlertText = $"Default Media Item set successfully.";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 128, 0));
                UpdateMediaAvailability();
                CheckForMedia(SelectedMediaType);
                OnPropertyChanged(nameof(ImageSource));
                await ShowAlertAsync();
            }
            catch (Exception e)
            {
                AlertText = $"Could not copy new media file to game media folder. Exception: {e.Message}";
                AlertColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 0, 0));
                await ShowAlertAsync(5000);
                return;
            }
        }
    }
}