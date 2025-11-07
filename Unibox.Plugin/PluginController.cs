using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Plugin.Services;
using Unibox.Plugin.ViewModels;
using Unibox.Plugin.Views;

namespace Unibox.Plugin
{
    internal sealed class PluginController
    {
        //public IServiceProvider Services { get; }

        private LaunchboxService _launchboxService;
        private MessagingService _messagingService;
        private LoggingService _loggingService;
        private GameMonitoringService _gameMonitoringService;

        private MainWindowVM _mainWindowVM = new MainWindowVM();

        private static readonly object _padlock = new object();

        private static PluginController _instance = null;

        public static PluginController Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new PluginController();
                    }
                    return _instance;
                }
            }
        }

        //private static IServiceProvider ConfigureServices()
        //{
        //    var services = new ServiceCollection();

        //    // Register Services
        //    services.AddSingleton<LaunchboxService>(); // Register the LaunchboxService
        //    services.AddSingleton<LoggingService>(); // Register the LoggingService
        //    services.AddSingleton<MessagingService>(); // Register the messageService

        //    // Register ViewModels
        //    //services.AddTransient<MainWindowVM>();

        //    // Add other necessary services here
        //    // e.g., services.AddSingleton<IDataService, DataService>();
        //    return services.BuildServiceProvider();
        //}

        public PluginController()
        {
            //Services = ConfigureServices();
            _loggingService = new LoggingService();
            _gameMonitoringService = new GameMonitoringService();

            _launchboxService = new LaunchboxService(_loggingService, _gameMonitoringService);
            _messagingService = new MessagingService(_launchboxService, _loggingService);
        }

        internal void ProcessSystemEvent(string eventType)
        {
            Debug.WriteLine($"[Unibox.Plugin] System Event Raised: {eventType}");
            switch (eventType)
            {
                case "PluginInitialized":
                    // Handle LaunchBox startup event
                    break;
            }
        }

        internal void ShowMainWindow()
        {
            MainWindow mainWindow = new MainWindow(_mainWindowVM);
            mainWindow.ShowDialog();
        }

        internal void ProcessGameLaunched(IGame game, IAdditionalApplication app, IEmulator emulator)
        {
            _gameMonitoringService.LastGameDetails = new Data.Models.GameLaunchDetails
            {
                Game = game,
                AdditionalApplication = app,
                Emulator = emulator,
            };
            _gameMonitoringService.GameCurrentlyPlaying = true;
        }

        internal void ProcessGameExited()
        {
            _gameMonitoringService.GameCurrentlyPlaying = false;
        }
    }
}