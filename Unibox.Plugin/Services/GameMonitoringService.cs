using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Plugin.Data.Models;

namespace Unibox.Plugin.Services
{
    internal class GameMonitoringService
    {
        public GameLaunchDetails LastGameDetails { get; set; } = null!;
        public bool GameCurrentlyPlaying { get; set; } = false;
    }
}