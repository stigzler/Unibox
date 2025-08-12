using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper;
using stigzler.ScreenscraperWrapper.Results;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;
using Unibox.Admin.Services;

namespace Unibox.Admin.ViewModels
{
    internal partial class MainWindowVM : ObservableObject
    {
        [ObservableProperty]
        private string consoleText = String.Empty;

        [ObservableProperty]
        private Cursor cursor = Cursors.Arrow;

        [ObservableProperty]
        private string lbRootPath = String.Empty;

        private ScreenscraperService screenscraperService;
        private SqliteService sqliteService;
        private DatabaseService databaseService;

        [ObservableProperty]
        private string ssApiName = String.Empty;

        [ObservableProperty]
        private string ssApiPassword = String.Empty;

        [ObservableProperty]
        private string ssApiUsername = String.Empty;

        [ObservableProperty]
        private string ssPassword = String.Empty;

        [ObservableProperty]
        private string ssUsername = String.Empty;

        [ObservableProperty]
        private string encryptKey = String.Empty;

        [ObservableProperty]
        private string encryptOutput = String.Empty;

        public MainWindowVM()
        {
        }

        public MainWindowVM(SqliteService sqliteService, ScreenscraperService screenscraperService, DatabaseService databaseService)
        {
            LoadSettings();

            this.sqliteService = sqliteService;
            this.screenscraperService = screenscraperService;
            this.databaseService = databaseService;

            SetSqliteFilepath();
            screenscraperService.UpdateCredentialsFromUserSettings();

            string encryptedString = Helpers.AESEncrypt.Encrypt("Dave Woz Ere", "passwordbaby");
            Debug.WriteLine($"Encrypted String: {encryptedString}");
            string decryptedString = Helpers.AESEncrypt.Decrypt(encryptedString, "passwordbaby");
            Debug.WriteLine($"Decrypted String: {decryptedString}");
        }

        [RelayCommand]
        private void EncryptScreenscraperCredentials()
        {
            EncryptOutput = String.Empty;
            EncryptOutput += EncryptKey + Environment.NewLine;
            EncryptOutput += Helpers.AESEncrypt.Encrypt(SsApiName, EncryptKey) + Environment.NewLine;
            EncryptOutput += Helpers.AESEncrypt.Encrypt(SsApiUsername, EncryptKey) + Environment.NewLine;
            EncryptOutput += Helpers.AESEncrypt.Encrypt(SsApiPassword, EncryptKey) + Environment.NewLine;
        }

        [RelayCommand]
        private void CopyText()
        {
            Clipboard.SetText(ConsoleText.TrimEnd(Environment.NewLine.ToCharArray()));
            AdonisUI.Controls.MessageBox.Show("Text copied to clipboard!", "Copy Text",
                AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);
        }

        [RelayCommand]
        private void GetLaunchboxMediaTypes()
        {
            var query = "SELECT DISTINCT Type FROM GameImages";
            var results = sqliteService.GetData(query);
            ConsoleText = String.Empty;
            // Handle results as needed, e.g., display in a UI element
            foreach (var mediaType in results)
            {
                ConsoleText += mediaType + Environment.NewLine;
            }
            ConsoleText += "Video" + Environment.NewLine;
            ConsoleText += "Music" + Environment.NewLine;
            ConsoleText += "Manual";
        }

        [RelayCommand]
        private async void GetScreenscraperRegions()
        {
            var dave = await screenscraperService.GetScreenscraperRegions();
            ConsoleText = String.Empty;
            foreach (var region in (List<Region>)dave.DataObject)
            {
                ConsoleText += $"{region.NameEnglish}|{region.NameShort}" + Environment.NewLine;
            }
        }

        [RelayCommand]
        private void GetLaunchboxPlatforms()
        {
            var query = "SELECT name FROM platforms";
            var results = sqliteService.GetData(query);

            ConsoleText = String.Empty;

            // Handle results as needed, e.g., display in a UI element
            foreach (var platform in results)
            {
                ConsoleText += platform + Environment.NewLine;
            }
        }

        [RelayCommand]
        private async Task GetScreenscraperSystems()
        {
            Cursor = Cursors.Wait;
            var dave = await screenscraperService.GetScreenscraperSystems();
            Cursor = Cursors.Arrow;

            ConsoleText = String.Empty;

            foreach (var system in (List<stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper.System>)dave.DataObject)
            {
                ConsoleText += $"{system.ID}|{system.NameEurope}" + Environment.NewLine;
            }
        }

        [RelayCommand]
        private void GetScreenscraperGameMediaTypeEnums()
        {
            ConsoleText = String.Empty;
            foreach (var enumName in Enum.GetValues(typeof(stigzler.ScreenscraperWrapper.Data.Enums.GameMediaType)))
            {
                ConsoleText += enumName.ToString() + Environment.NewLine;
            }
        }

        private void LoadSettings()
        {
            LbRootPath = Properties.Settings.Default.LbInstallationRootPath;
            //SsUsername = Properties.Settings.Default.SsUsername;
            //SsPassword = Properties.Settings.Default.SsPassword;
            //SsApiName = Properties.Settings.Default.ssApiName;
            //SsApiUsername = Properties.Settings.Default.ssApiUsername;
            //SsApiPassword = Properties.Settings.Default.ssApiPassword;

            SsUsername = "Dummy";
            SsPassword = "Dummy";
            SsApiName = "Dummy";
            SsApiUsername = "Dummy";
            SsApiPassword = "Dummy";
        }

        [RelayCommand]
        private void SaveToSettings()
        {
            Properties.Settings.Default.LbInstallationRootPath = LbRootPath;
            Properties.Settings.Default.SsUsername = SsUsername;
            Properties.Settings.Default.SsPassword = SsPassword;
            Properties.Settings.Default.ssApiName = SsApiName;
            Properties.Settings.Default.ssApiUsername = SsApiUsername;
            Properties.Settings.Default.ssApiPassword = SsApiPassword;

            SetSqliteFilepath();
            screenscraperService.UpdateCredentialsFromUserSettings();
        }

        private void SetSqliteFilepath()
        {
            sqliteService.Filepath = System.IO.Path.Combine(Properties.Settings.Default.LbInstallationRootPath,
                @"Metadata\LaunchBox.Metadata.db");
        }
    }
}