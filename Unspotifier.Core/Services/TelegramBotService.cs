using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Unspotifier.Core.Models;

namespace Unspotifier.Core.Services
{
    public class TelegramBotService
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettings _applicationSettings;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly UnspotifyService _unspotifyService;

        public TelegramBotService(ILogger logger, ApplicationSettings applicationSettings, ITelegramBotClient telegramBotClient, UnspotifyService unspotifyService)
        {
            _logger = logger;
            _applicationSettings = applicationSettings;
            _telegramBotClient = telegramBotClient;
            _unspotifyService = unspotifyService;

            _telegramBotClient.OnMessage += TelegramBotClientOnOnMessage;
        }

        public void Start()
        {
            _telegramBotClient.StartReceiving();
        }

        public void Stop()
        {
            _telegramBotClient.StopReceiving();
        }

        private async void TelegramBotClientOnOnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {
                var message = messageEventArgs.Message;

                if (message == null || message.Type != MessageType.Text) return;

                switch (message.Text.Split(' ').First())
                {
                    case "/unspotify":
                        await ((TelegramBotClient)sender).SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                        var userMessage = message.ReplyToMessage?.Text ?? message.Text.Split(' ').Last();
                        var resultMarkdown = await _unspotifyService.UnspotifyUri(userMessage);
                        if (resultMarkdown.Length > _applicationSettings.TelegramMaxMessageLength)
                        {
                            resultMarkdown = resultMarkdown.Remove(_applicationSettings.TelegramMaxMessageLength);
                        }
                        await ((TelegramBotClient)sender).SendTextMessageAsync(message.Chat.Id, resultMarkdown, ParseMode.Markdown, true, true, messageEventArgs.Message.MessageId);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $@"Error during {nameof(TelegramBotClientOnOnMessage)}");
            }
        }
    }
}
