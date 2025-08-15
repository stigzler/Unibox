using Bluegrams.Application;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Unibox.Plugin.Services;

namespace Unibox.Plugin
{
    public class Main
    {
        public Main()
        {
            PortableSettingsProvider.SettingsFileName = Path.Combine(
               Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "unibox.plugin.config");

            PortableSettingsProvider.ApplyProvider(Properties.Settings.Default);

            Services = ConfigureServices();
            MessagingService messageService = (MessagingService)Services.GetService(typeof(MessagingService));
        }

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
            services.AddSingleton<LaunchboxService>(); // Register the LaunchboxService
            services.AddSingleton<MessagingService>(); // Register the messageService

            // Add other necessary services here
            // e.g., services.AddSingleton<IDataService, DataService>();
            return services.BuildServiceProvider();
        }
    }
}