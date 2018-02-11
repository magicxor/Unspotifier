using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unspotifier.DependencyInjection;
using Unspotifier.Models;
using Unspotifier.Services;

namespace Unspotifier
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                var configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                var configurationRoot = configurationBuilder.Build();
                var applicationSettings = configurationRoot.GetSection("UnspotifierConfiguration").Get<ApplicationSettings>();

                var serviceCollection = new ServiceCollection();
                ContainerConfiguration.ConfigureServices(serviceCollection, applicationSettings);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var botService = serviceProvider.GetService<TelegramBotService>();
                var logger = serviceProvider.GetService<ILogger>();

                logger.LogInformation("Running...");
                Console.ReadLine();
                logger.LogInformation("Shutdown...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
