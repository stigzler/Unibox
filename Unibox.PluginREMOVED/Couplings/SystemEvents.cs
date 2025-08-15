using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Plugin.Helpers;

namespace Unibox.Plugin.Couplings
{
    internal class SystemEvents : ISystemEventsPlugin

    {
        public void OnEventRaised(string eventType)
        {
            switch (eventType)
            {
                case SystemEventTypes.PluginInitialized:
                    Log.WriteLine("Plugin Initialised. Starting log...");
                    Main main = new Main();
                    break;

                default:
                    // Handle other events if necessary
                    break;
            }
        }
    }
}