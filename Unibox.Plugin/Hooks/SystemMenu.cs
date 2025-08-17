using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Plugin.ViewModels;
using Unibox.Plugin.Views;

namespace Unibox.Plugin.Hooks
{
    internal class SystemMenu : ISystemMenuItemPlugin
    {
        // Carefull with accessing Settings here - Not implemented unitl Main
        // see PortableSettingProvider: https://github.com/Bluegrams/SettingsProviders

        public string Caption => "Unibox Settings";

        public System.Drawing.Image IconImage => Properties.Resources.UniboxLogo64px;

        public bool ShowInLaunchBox => true;

        public bool ShowInBigBox => false;

        public bool AllowInBigBoxWhenLocked => false;

        public void OnSelected()
        {
            MainWindowVM mainWindowVM = new MainWindowVM();
            MainWindow mainWindow = new MainWindow(mainWindowVM);

            mainWindow.ShowDialog();
        }
    }
}