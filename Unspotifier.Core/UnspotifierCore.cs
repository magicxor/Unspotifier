using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unspotifier.Core.Constants;
using Unspotifier.Core.DependencyInjection;
using Unspotifier.Core.Models;
using Unspotifier.Core.Services;

namespace Unspotifier.Core
{
    public static class UnspotifierCore
    {
        public static void Run()
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

                botService.Start();
                logger.LogInformation($"{nameof(UnspotifierCore)} started");
                Console.ReadLine();
                botService.Stop();
                logger.LogInformation($"{nameof(UnspotifierCore)} stopped");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
