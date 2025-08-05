using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using Unibox.Admin.ViewModels;

namespace Unibox.Admin
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

            Window window = new MainWindow
            {
                DataContext = Services.GetService<MainWindowVM>() // Set the DataContext to the MainWindowVM
            };

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
            services.AddSingleton<Admin.Services.SqliteService>();

            // Register ViewModels
            services.AddTransient<MainWindowVM>();

            // Add other necessary services here
            // e.g., services.AddSingleton<IDataService, DataService>();
            return services.BuildServiceProvider();
        }
    }
}