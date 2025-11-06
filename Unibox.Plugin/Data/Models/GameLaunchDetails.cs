using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins.Data;

namespace Unibox.Plugin.Data.Models
{
    internal class GameLaunchDetails
    {
        public IGame Game { get; set; } = null!;
        public IAdditionalApplication AdditionalApplication { get; set; } = null!;
        public IEmulator Emulator { get; set; } = null!;
        //public DateTime TimeLaunched { get; set; } = DateTime.Now;
    }
}