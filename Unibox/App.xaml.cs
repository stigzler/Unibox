using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Unibox.Properties;
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
        private Window window;

        public App()
        {
            Services = ConfigureServices();

            this.InitializeComponent();

            UpgradeSettingsIfNeeded();
        }

        private void UpgradeSettingsIfNeeded()
        {
            string configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            if (!File.Exists(configPath))
            {
                //Existing user config does not exist, so load settings from previous assembly
                Settings.Default.Upgrade();
                Settings.Default.Reload();
                Settings.Default.Save();
            }
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
            services.AddSingleton<LoggingService>(); // Register the logging service
            services.AddSingleton<GameService>(); // Register the game service

            // Register ViewModels
            services.AddTransient<MainWindowVM>();
            services.AddTransient<SettingsVM>();
            services.AddTransient<GamesVM>();
            services.AddTransient<InstallationsVM>();
            services.AddTransient<EditInstallationVM>();
            services.AddTransient<EditInstallationPlatformsVM>();
            services.AddTransient<PleaseWaitVM>();
            services.AddTransient<AddGameResultsVM>();
            services.AddTransient<UpdatePlatformsResultsVM>();

            // Add other necessary services here
            // e.g., services.AddSingleton<IDataService, DataService>();
            return services.BuildServiceProvider();
        }

        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "UniBox";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                //window.Hide();

                Helpers.Theming.ApplyTheme();

                AdonisUI.Controls.MessageBox.Show($"Another instance of Unibox is already running. Please switch to that instance.", "Unibox already running",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);

                //app is already running! Exiting the application
                Application.Current.Shutdown();
            }

            window = new MainWindow();

            window.Show();

            base.OnStartup(e);
        }
    }
}