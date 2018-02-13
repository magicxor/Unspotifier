using System;
using System.IO;
using System.Reflection;
using FluentSpotifyApi.AuthorizationFlows.ClientCredentials;
using FluentSpotifyApi.AuthorizationFlows.ClientCredentials.Extensions;
using FluentSpotifyApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Unspotifier.Core.Models;
using Unspotifier.Core.Services;

namespace Unspotifier.Core.DependencyInjection
{
    public static class ContainerConfiguration
    {
        public static void ConfigureServices(IServiceCollection services, ApplicationSettings applicationSettings)
        {
            services.Configure<ClientCredentialsFlowOptions>(options =>
            {
                options.ClientId = applicationSettings.SpotifyApiClientId;
                options.ClientSecret = applicationSettings.SpotifyApiClientSecret;
                options.TokenEndpoint = new Uri(applicationSettings.SpotifyApiAuthorizationUri);
                options.Validate();
            });

            var loggerFactory = new LoggerFactory();
            var logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), applicationSettings.LogDirectoryName, "log-{Date}.txt");
            loggerFactory.AddConsole(LogLevel.Trace).AddFile(logPath, LogLevel.Trace, retainedFileCountLimit: 10);

            services
                .AddScoped<ILogger>(provider => loggerFactory.CreateLogger(""))
                .AddSingleton<ApplicationSettings>(applicationSettings)
                .AddFluentSpotifyClient(clientBuilder => clientBuilder.ConfigurePipeline(pipeline => pipeline.AddClientCredentialsFlow()))
                .AddSingleton<ITelegramBotClient>(new TelegramBotClient(applicationSettings.TelegramBotToken))
                .AddSingleton<TelegraphPublisher>()
                .AddSingleton<TelegramBotService>()
                .AddSingleton<SpotifyUriParser>()
                .AddSingleton<UnspotifyService>();
        }
    }
}
