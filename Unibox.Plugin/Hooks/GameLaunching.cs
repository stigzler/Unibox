using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace Unibox.Plugin.Hooks
{
    internal class GameLaunching : IGameLaunchingPlugin
    {
        public void OnAfterGameLaunched(IGame game, IAdditionalApplication app, IEmulator emulator)
        {
            PluginController.Instance.ProcessGameLaunched(game, app, emulator);
            // throw new NotImplementedException();
        }

        public void OnBeforeGameLaunching(IGame game, IAdditionalApplication app, IEmulator emulator)
        {
            // throw new NotImplementedException();
        }

        public void OnGameExited()
        {
            PluginController.Instance.ProcessGameExited();
        }
    }
}