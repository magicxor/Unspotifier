using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unspotifier.Core.Constants;
using Unspotifier.DependencyInjection;
using Unspotifier.Models;
using Unspotifier.Services;

namespace Unspotifier
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(Defaults.ConfigurationFileName);
                var configurationRoot = configurationBuilder.Build();
                var applicationSettings = configurationRoot.GetSection(Defaults.ConfigurationSectionName).Get<ApplicationSettings>();

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
