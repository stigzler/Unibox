using AdonisUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetMessage;
using NetMessage.Base;
using stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper;
using stigzler.ScreenscraperWrapper.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Messaging.Requests;
using Unibox.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace Unibox.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        [ObservableProperty]
        private string ssApiName = string.Empty;

        [ObservableProperty]
        private string ssApiPassword = string.Empty;

        [ObservableProperty]
        private string ssApiUsername = string.Empty;

        [ObservableProperty]
        private string ssPassword = string.Empty;

        [ObservableProperty]
        private string ssUsername = string.Empty;

        [ObservableProperty]
        private Cursor cursor = Cursors.Arrow;

        [ObservableProperty]
        private string assemblyVersion = $"V{Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "{irretrievable}"}";

        [ObservableProperty]
        private string selectedRegion;

        private ScreenscraperService screenscraperService;

        public SettingsVM()
        {
        }

        public SettingsVM(ScreenscraperService screenscraperService)
        {
            this.screenscraperService = screenscraperService;
            LoadSettings();
        }

        [RelayCommand]
        private async void Test()
        {
        }

        /// <summary>
        /// Note: some settings are loaded/updated in the XAML directly so won't be here.
        /// </summary>
        private void LoadSettings()
        {
            SsUsername = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsUsername);
            SsPassword = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsPassword);
            SsApiName = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsApiName);
            SsApiUsername = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsApiUsername);
            SsApiPassword = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsApiPassword);
        }

        [RelayCommand]
        private void MoveSelectedRegionToTop()
        {
            Properties.Settings.Default.SsRegionPriorities.Remove(SelectedRegion);
            Properties.Settings.Default.SsRegionPriorities.Insert(0, SelectedRegion);
            Properties.Settings.Default.Save();
            CollectionViewSource.GetDefaultView(Properties.Settings.Default.SsRegionPriorities).Refresh();
        }

        [RelayCommand]
        private void MoveSelectedRegionUp()
        {
            int index = Properties.Settings.Default.SsRegionPriorities.IndexOf(SelectedRegion);
            if (index > 0)
            {
                Properties.Settings.Default.SsRegionPriorities.Remove(SelectedRegion);
                Properties.Settings.Default.SsRegionPriorities.Insert(index - 1, SelectedRegion);
                Properties.Settings.Default.Save();
                CollectionViewSource.GetDefaultView(Properties.Settings.Default.SsRegionPriorities).Refresh();
            }
        }

        [RelayCommand]
        private void MoveSelectedRegionDown()
        {
            int index = Properties.Settings.Default.SsRegionPriorities.IndexOf(SelectedRegion);
            if (index < Properties.Settings.Default.SsRegionPriorities.Count - 1)
            {
                Properties.Settings.Default.SsRegionPriorities.Remove(SelectedRegion);
                Properties.Settings.Default.SsRegionPriorities.Insert(index + 1, SelectedRegion);
                Properties.Settings.Default.Save();
                CollectionViewSource.GetDefaultView(Properties.Settings.Default.SsRegionPriorities).Refresh();
            }
        }

        [RelayCommand]
        private void ToggleDarkMode()
        {
            Helpers.Theming.ApplyTheme();
        }

        [RelayCommand]
        private async void TestScreenscraperConnection()
        {
            SaveScreenscraperCredentials();
            Mouse.OverrideCursor = Cursors.Wait;
            ApiGetDataOutcome outcome = await screenscraperService.GetUser();
            Mouse.OverrideCursor = Cursors.Arrow;
            if (outcome.OverallOutcome == stigzler.ScreenscraperWrapper.Data.Enums.OverallOutcome.FullySuccessful)
            {
                User user = outcome.DataObject as User;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Connection successful. Your user details:");
                sb.AppendLine($"Username: {user.Username}");
                sb.AppendLine($"Max Threads: {user.MaxThreads}");
                sb.AppendLine($"Total requests today: {user.TotalRequestsToday}");

                AdonisUI.Controls.MessageBox.Show(sb.ToString(), "Connection Successful",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);

                // Update User threads
                Properties.Settings.Default.ssThreads = user.MaxThreads;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Connection unsuccessful. Details:");
                sb.AppendLine($"Overall Outcome: {outcome.OverallOutcome}");
                if (outcome.HttpStatusCode != null) sb.AppendLine($"Http Status Code: {outcome.HttpStatusCode}");
                if (outcome.TextResult != null) sb.AppendLine($"Screenscraper text: {outcome.TextResult}");
                if (outcome.Exception != null) sb.AppendLine($"Exception: {outcome.Exception.Message}");

                AdonisUI.Controls.MessageBox.Show(sb.ToString(), "Connection Unsuccessful",
                AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void SaveScreenscraperCredentials()
        {
            Properties.Settings.Default.SsPassword = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsPassword);
            Properties.Settings.Default.SsUsername = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsUsername);

            if (String.IsNullOrWhiteSpace(SsApiName)) Properties.Settings.Default.SsApiName = string.Empty;
            else Properties.Settings.Default.SsApiName = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsApiName);

            if (String.IsNullOrWhiteSpace(SsApiPassword)) Properties.Settings.Default.SsApiPassword = string.Empty;
            else Properties.Settings.Default.SsApiPassword = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsApiPassword);

            if (String.IsNullOrWhiteSpace(SsApiUsername)) Properties.Settings.Default.SsApiUsername = string.Empty;
            else Properties.Settings.Default.SsApiUsername = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsApiUsername);

            Properties.Settings.Default.Save();

            screenscraperService.UpdateCredentialsFromUserSettings();
            // AdonisUI.Controls.MessageBox.Show("Screenscraper credentials saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}