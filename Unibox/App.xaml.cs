using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
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
        private LoggingService loggingService;

        public App()
        {
            Services = ConfigureServices();

            this.InitializeComponent();

            UpgradeSettingsIfNeeded();

            loggingService = Services.GetService<LoggingService>();

            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;

            HandleUserDataSetup();
        }

        private void HandleUserDataSetup()
        {
            // ensure AppData folder exists
            if (!Directory.Exists(Data.Constants.Paths.LocalAppDataDir))
            {
                Directory.CreateDirectory(Data.Constants.Paths.LocalAppDataDir);
            }

            // This handles upgrading form 0.9.x to 1.0.0 where the litedb file moved from the app root
            // to the AppData folder - moves existing file if found

            if (File.Exists(Path.Combine(AppContext.BaseDirectory, "Unibox.ldb")) &&
                !File.Exists(Path.Combine(Data.Constants.Paths.LocalAppDataDir, "Unibox.ldb")))
            {
                File.Move(Path.Combine(AppContext.BaseDirectory, "Unibox.ldb"),
                    Path.Combine(Data.Constants.Paths.LocalAppDataDir, "Unibox.ldb"));
            }
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string logEntry = $"Unhandled exception: {e.Exception.Message}\r\nStack Trace: {e.Exception.StackTrace}";

            loggingService.WriteLine(logEntry);

            string errorMessage = string.Format("An unhandled exception occurred. See logs for more details and berate the author. Exception: \r\n\r\n{0}", e.Exception.Message);

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow; // this here in case cursor set to wait in crashed process

            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
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
            services.AddTransient<EditPlatformVM>();
            services.AddTransient<EditGameVM>();

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