using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Plugin.Helpers;

namespace Unibox.Plugin.Hooks
{
    internal class SystemEvents : ISystemEventsPlugin
    {
        // Carefull with accessing Settings here - Not implemented unitl Main
        // see PortableSettingProvider: https://github.com/Bluegrams/SettingsProviders

        public void OnEventRaised(string eventType)
        {
            switch (eventType)
            {
                case SystemEventTypes.PluginInitialized:
                    // Handle LaunchBox startup event
                    Main main = new Main();
                    break;
            }
        }
    }
}