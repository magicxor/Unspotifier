using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSpotifyApi;
using FluentSpotifyApi.Model;
using Unspotifier.Core.Enums;

namespace Unspotifier.Core.Services
{
    public class UnspotifyService
    {
        private readonly SpotifyUriParser _spotifyUriParser;
        private readonly IFluentSpotifyClient _fluentSpotifyClient;

        public UnspotifyService(SpotifyUriParser spotifyUriParser, IFluentSpotifyClient fluentSpotifyClient)
        {
            _spotifyUriParser = spotifyUriParser;
            _fluentSpotifyClient = fluentSpotifyClient;
        }

        private string GetGoogleSearchUri(string query)
        {
            return $"https://www.google.com/search?q={query}";
        }

        private string GetYoutubeSearchUri(string query)
        {
            return $"https://www.youtube.com/results?search_query={query}";
        }

        private string GetYandexMusicSearchUri(string query)
        {
            return $"https://music.yandex.ru/search?text={query}";
        }

        private string GetLastFmSearchUri(string query)
        {
            return $"https://www.last.fm/search?q={query}";
        }

        private string GetVkSearchUri(string query)
        {
            return $"https://vk.com/audios0?q={query}";
        }

        private string GetArtistsString(IEnumerable<SimpleArtist> artist)
        {
            return string.Join(", ", artist.Select(x => x.Name));
        }

        private string GetCopyrightsString(IEnumerable<Copyright> copyrights)
        {
            return string.Join(", ", copyrights.Select(x => x.Text));
        }

        private string GetTrackDurationString(int durationMs)
        {
            return TimeSpan.FromMilliseconds(durationMs).ToString(@"mm\:ss");
        }

        private string GetTrackPreviewButtonString(FullTrack track)
        {
            return string.IsNullOrEmpty(track.PreviewUrl)
                ? "🔇"
                : $"[▶]({track.PreviewUrl})";
        }

        private string GetTrackPreviewButtonString(SimpleTrack track)
        {
            return string.IsNullOrEmpty(track.PreviewUrl)
                ? "🔇"
                : $"[▶]({track.PreviewUrl})";
        }

        private string GetSearchString(string firstQueryPart, string secondQueryPart = null)
        {
            var query = secondQueryPart == null ? firstQueryPart : $"{firstQueryPart} - {secondQueryPart}";
            var encodedQuery = System.Net.WebUtility.UrlEncode(query);
            if (!string.IsNullOrEmpty(encodedQuery))
            {
                return encodedQuery?.Replace("(", string.Empty)?.Replace(")", string.Empty);
            }
            else
            {
                return string.Empty;
            }
        }

        private string GenerateMarkdownForAllSearchEngines(string searchQuery)
        {
            return $@"[(Google)]({GetGoogleSearchUri(searchQuery)}) [(YouTube)]({GetYoutubeSearchUri(searchQuery)}) [(ЯндексМузыка)]({GetYandexMusicSearchUri(searchQuery)}) [(LastFM)]({GetLastFmSearchUri(searchQuery)}) [(VK)]({GetVkSearchUri(searchQuery)})";
        }

        private string GenerateMarkdown(FullTrack track)
        {
            var artists = GetArtistsString(track.Artists);
            var searchQuery = GetSearchString(artists, track.Name);
            var previewButtonString = GetTrackPreviewButtonString(track);
            var durationString = GetTrackDurationString(track.DurationMs);

            var sb = new StringBuilder();
            sb.AppendLine($"{artists} [{track.Album.Name}] - {track.TrackNumber}. {track.Name} ({durationString})");
            sb.AppendLine($"{previewButtonString} {GenerateMarkdownForAllSearchEngines(searchQuery)}");
            sb.AppendLine($"track popularity: {track.Popularity}");

            return sb.ToString();
        }

        private string GenerateMarkdown(FullAlbum album)
        {
            var artists = GetArtistsString(album.Artists);
            var searchQuery = GetSearchString(artists, album.Name);

            var sb = new StringBuilder();
            sb.AppendLine($"{artists} - {album.Name}");
            sb.AppendLine($"{GenerateMarkdownForAllSearchEngines(searchQuery)}");
            sb.AppendLine($"album popularity: {album.Popularity} | tracks: {album.Tracks.Total} | genres: {string.Join(", ", album.Genres)} | release date: {album.ReleaseDate} | copyrights: {GetCopyrightsString(album.Copyrights)}");
            sb.AppendLine();

            foreach (var track in album.Tracks.Items)
            {
                var trackSearchQuery = GetSearchString(artists, track.Name);
                var previewButtonString = GetTrackPreviewButtonString(track);
                sb.AppendLine($"{track.TrackNumber} - {track.Name} ({GetTrackDurationString(track.DurationMs)}) {previewButtonString} [🔎]({GetGoogleSearchUri(trackSearchQuery)})");
            }

            return sb.ToString();
        }

        private string GenerateMarkdown(FullArtist artist)
        {
            var searchQuery = GetSearchString(artist.Name);

            var sb = new StringBuilder();
            sb.AppendLine($"{artist.Name}");
            sb.AppendLine($"{GenerateMarkdownForAllSearchEngines(searchQuery)}");
            sb.AppendLine($"artist popularity: {artist.Popularity} | genres: {string.Join(", ", artist.Genres)}");

            return sb.ToString();
        }

        private string GenerateMarkdown(IEnumerable<PlaylistTrack> playlistTracks)
        {
            var sb = new StringBuilder();
            foreach (var playlistTrack in playlistTracks)
            {
                var artists = GetArtistsString(playlistTrack.Track.Artists);
                var searchQuery = GetSearchString(artists, playlistTrack.Track.Name);
                var googleSearchUri = GetGoogleSearchUri(searchQuery);
                var previewButtonString = GetTrackPreviewButtonString(playlistTrack.Track);
                var durationString = GetTrackDurationString(playlistTrack.Track.DurationMs);

                sb.AppendLine($"{artists} - {playlistTrack.Track.Name} ({durationString}) {previewButtonString} [🔎]({googleSearchUri})");
            }
            return sb.ToString();
        }

        public async Task<string> UnspotifyUri(string uri)
        {
            var uriType = _spotifyUriParser.GetUriType(uri);
            switch (uriType)
            {
                case SpotifyUriTypes.Track:
                    var trackId = _spotifyUriParser.ParseTrackUri(uri);
                    var track = await _fluentSpotifyClient.Track(trackId).GetAsync();
                    return GenerateMarkdown(track);
                case SpotifyUriTypes.Album:
                    var albumId = _spotifyUriParser.ParseAlbumUri(uri);
                    var album = await _fluentSpotifyClient.Album(albumId).GetAsync();
                    return GenerateMarkdown(album);
                case SpotifyUriTypes.Artist:
                    var artistId = _spotifyUriParser.ParseArtistUri(uri);
                    var artist = await _fluentSpotifyClient.Artist(artistId).GetAsync();
                    return GenerateMarkdown(artist);
                case SpotifyUriTypes.Playlist:
                    var playlistUriParts = _spotifyUriParser.ParsePlaylistUri(uri);
                    var tracks = await _fluentSpotifyClient.User(playlistUriParts.UserId).Playlist(playlistUriParts.PlaylistId).Tracks().GetAsync();
                    return GenerateMarkdown(tracks.Items);
                default:
                    throw new ArgumentOutOfRangeException(nameof(uriType), $"Unknown {nameof(uriType)}: {uriType} got from {uri}");
            }
        }
    }
}
