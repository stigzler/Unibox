﻿using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using Unibox.Services;
using Unibox.ViewModels;
using Unibox.Views;

namespace Unibox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();

            this.InitializeComponent();

            Helpers.Log.StartLog();

            Window window = new MainWindow();
            window.Show();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register Services
            services.AddSingleton<DatabaseService>(); // Register the databaseService
            services.AddSingleton<InstallationService>(); // Register the installation service
            services.AddSingleton<PlatformService>(); // Register the platform update service
            services.AddSingleton<FileService>(); // Register the settings service
            services.AddSingleton<ScreenscraperService>(); // Register the screenscraper service
            services.AddSingleton<MessagingService>(); // Register the messaging service
            services.AddSingleton<GameService>(); // Register the game service

            // Register ViewModels
            services.AddTransient<MainWindowVM>();
            services.AddTransient<SettingsVM>();
            services.AddTransient<GamesVM>();
            services.AddTransient<InstallationsVM>();
            services.AddTransient<EditInstallationVM>();
            services.AddTransient<InstallationPlatformDetailsVM>();
            services.AddTransient<PleaseWaitVM>();
            services.AddTransient<AddGameResultsVM>();
            services.AddTransient<UpdatePlatformsResultsVM>();

            // Add other necessary services here
            // e.g., services.AddSingleton<IDataService, DataService>();
            return services.BuildServiceProvider();
        }
    }
}