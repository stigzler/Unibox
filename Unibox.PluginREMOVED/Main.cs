using Microsoft.Extensions.DependencyInjection;
using Unibox.Plugin.Services;

namespace Unibox.Plugin
{
    public class Main
    {
        public Main()
        {
            Services = ConfigureServices();

            MessageService messageService = (MessageService)Services.GetService(typeof(MessageService));
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
            services.AddSingleton<MessageService>(); // Register the messageService
            services.AddSingleton<LaunchboxService>(); // Register the LaunchboxService

            // Add other necessary services here
            // e.g., services.AddSingleton<IDataService, DataService>();
            return services.BuildServiceProvider();
        }
    }
}