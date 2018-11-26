using HomeMonitor.Options;
using HomeMonitor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace HomeMonitor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetService<IApp>();
            await app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Logging
            services.AddSingleton(
                new LoggerFactory()
                    .AddConsole()
                    .AddDebug()
            );
            services.AddLogging();

            // Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddOptions();
            services.Configure<NestOptions>(configuration.GetSection("Nest"));

            // Services
            services.AddTransient<IApp, App>();
            services.AddSingleton<INestApiClient, NestApiClient>();
            services.AddSingleton<IStatsExporter, PrometheusExporter>();
        }
    }
}
