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

        partial void OnSsApiNameChanged(string value)
        {
            Properties.Settings.Default.SsApiName = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(value);
        }

        partial void OnSsApiPasswordChanged(string value)
        {
            Properties.Settings.Default.SsApiPassword = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(value);
        }

        partial void OnSsApiUsernameChanged(string value)
        {
            Properties.Settings.Default.SsApiUsername = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(value);
        }

        partial void OnSsPasswordChanged(string value)
        {
            Properties.Settings.Default.SsPassword = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(value);
        }

        partial void OnSsUsernameChanged(string value)
        {
            Properties.Settings.Default.SsUsername = Helpers.Encryption.DpapiEncrypt.QuickEncrypt(value);
        }

        [RelayCommand]
        private void ToggleDarkMode()
        {
            Helpers.Theming.ApplyTheme();
        }
    }
}