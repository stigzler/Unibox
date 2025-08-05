using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Unibox.Admin.ViewModels
{
    internal partial class MainWindowVM : ObservableObject
    {
        private Services.SqliteService sqliteService;

        [ObservableProperty]
        private string lbRootPath = String.Empty;

        [ObservableProperty]
        private string ssUsername = String.Empty;

        [ObservableProperty]
        private string ssPassword = String.Empty;

        [ObservableProperty]
        private string consoleText = String.Empty;

        [RelayCommand]
        private void SaveToSettings()
        {
            Properties.Settings.Default.LbInstallationRootPath = LbRootPath;
            Properties.Settings.Default.SsUsername = SsUsername;
            Properties.Settings.Default.SsPassword = SsPassword;

            SetSqliteFilepath();
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
        }

        [RelayCommand]
        private void CopyText()
        {
            Clipboard.SetText(ConsoleText.TrimEnd(Environment.NewLine.ToCharArray()));
            AdonisUI.Controls.MessageBox.Show("Text copied to clipboard!", "Copy Text",
                AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);
        }

        private void LoadSettings()
        {
            LbRootPath = Properties.Settings.Default.LbInstallationRootPath;
            SsUsername = Properties.Settings.Default.SsUsername;
            SsPassword = Properties.Settings.Default.SsPassword;
        }

        private void SetSqliteFilepath()
        {
            sqliteService.Filepath = System.IO.Path.Combine(Properties.Settings.Default.LbInstallationRootPath,
                @"Metadata\LaunchBox.Metadata.db");
        }

        public MainWindowVM(Admin.Services.SqliteService sqliteService)
        {
            LoadSettings();
            this.sqliteService = sqliteService;
            SetSqliteFilepath();
        }
    }
}