using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unibox.Plugin.Services;

namespace Unibox.Plugin.ViewModels
{
    public partial class MainWindowVM : ObservableObject, IDataErrorInfo
    {
        [ObservableProperty]
        private string port = Properties.Settings.Default.Port.ToString();

        [ObservableProperty]
        private bool isValid = true;

        private static readonly string[] ValidatedProperties = { nameof(Port) };

        internal MainWindowVM()
        {
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string validaitonError = ProcessPropertyChange(propertyName);
                ValidateForm();
                return validaitonError;
            }
        }

        private string ProcessPropertyChange(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Port):
                    if (string.IsNullOrWhiteSpace(Port))
                        return "Port must contain a number";
                    else if (!Port.All(char.IsDigit))
                        return "Port must be a number.";
                    else if (Convert.ToInt32(Port) < 49152 || Convert.ToInt32(Port) > 65535)
                        return "Port must be a number between 49152 and 65535";
                    else
                        break;
            }

            return null;
        }

        public void ValidateForm()
        {
            foreach (string property in ValidatedProperties)
            {
                if (ProcessPropertyChange(property) != null)
                {
                    IsValid = false;
                    return;
                }
            }
            IsValid = true;
        }

        public string Error => throw new NotImplementedException();

        [RelayCommand]
        private void SaveSettings(Window window)
        {
            if (IsValid)
            {
                Properties.Settings.Default.Port = Convert.ToInt32(Port);
                Properties.Settings.Default.Save();
                if (window != null) window.Close();
            }
        }

        [RelayCommand]
        private void Test()
        {
            // _messagingService.
        }
    }
}