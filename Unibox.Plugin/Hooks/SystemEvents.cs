using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Plugin.Helpers;

namespace Unibox.Plugin.Hooks
{
    internal class SystemEvents : ISystemEventsPlugin
    {
        public void OnEventRaised(string eventType)
        {
            switch (eventType)
            {
                case SystemEventTypes.PluginInitialized:
                    // Handle LaunchBox startup event
                    Log.StartLog();
                    Log.WriteLine("Plugin initialized. Starting server..");
                    Main main = new Main();
                    break;
            }
        }
    }
}