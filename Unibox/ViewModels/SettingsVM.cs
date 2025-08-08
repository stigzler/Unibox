using AdonisUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unibox.Services;

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

        private ScreenscraperService screenscraperService;

        public SettingsVM()
        {
        }

        public SettingsVM(ScreenscraperService screenscraperService)
        {
            this.screenscraperService = screenscraperService;
            LoadSettings();
        }

        /// <summary>
        /// Note: some settings are loaded/updated in the XAML directly so won't be here.
        /// </summary>
        private void LoadSettings()
        {
            ssUsername = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsUsername);
            ssPassword = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsPassword);
        }

        [RelayCommand]
        private void ToggleDarkMode()
        {
            Helpers.Theming.ApplyTheme();
        }

        [RelayCommand]
        private void SaveScreenscraperCredentials()
        {
            Properties.Settings.Default.SsPassword = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsPassword);
            Properties.Settings.Default.SsUsername = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsUsername);

            if (!String.IsNullOrWhiteSpace(SsApiName)) Properties.Settings.Default.SsApiName = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsApiName);
            if (!String.IsNullOrWhiteSpace(SsApiPassword)) Properties.Settings.Default.SsApiPassword = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsApiPassword);
            if (!String.IsNullOrWhiteSpace(SsApiUsername)) Properties.Settings.Default.SsApiUsername = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(SsApiUsername);

            Properties.Settings.Default.Save();

            screenscraperService.UpdateCredentialsFromUserSettings();
            // AdonisUI.Controls.MessageBox.Show("Screenscraper credentials saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}