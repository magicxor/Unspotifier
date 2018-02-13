namespace Unspotifier.Core.Models
{
    public class ApplicationSettings
    {
        public string SpotifyApiBaseUri { get; set; } = "https://api.spotify.com";
        public string SpotifyShareBaseUri { get; set; } = "https://open.spotify.com";
        public string SpotifyApiAuthorizationUri { get; set; } = "https://accounts.spotify.com/api/token";
        public string SpotifyApiClientId { get; set; }
        public string SpotifyApiClientSecret { get; set; }
        public string TelegramBotToken { get; set; }
        public int TelegramMaxMessageLength { get; set; } = 4096;
        public string TelegraphAccessToken { get; set; }
        public string LogDirectoryName { get; set; } = "logs";
    }
}
