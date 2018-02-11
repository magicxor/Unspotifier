using System;
using System.Text.RegularExpressions;
using Unspotifier.Enums;
using Unspotifier.Models;

namespace Unspotifier.Services
{
    public class SpotifyUriParser
    {
        private readonly ApplicationSettings _applicationSettings;
        
        private readonly Regex _trackUriRegex;
        private readonly Regex _albumUriRegex;
        private readonly Regex _artistUriRegex;
        private readonly Regex _playlistUriRegex;

        public SpotifyUriParser(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;

            _trackUriRegex = new Regex(Regex.Escape(_applicationSettings.SpotifyShareBaseUri) + @"/track/([^/?]+)[^/\r\n]*", RegexOptions.Compiled);
            _albumUriRegex = new Regex(Regex.Escape(_applicationSettings.SpotifyShareBaseUri) + @"/album/([^/?]+)[^/\r\n]*", RegexOptions.Compiled);
            _artistUriRegex = new Regex(Regex.Escape(_applicationSettings.SpotifyShareBaseUri) + @"/artist/([^/?]+)[^/\r\n]*", RegexOptions.Compiled);
            _playlistUriRegex = new Regex(Regex.Escape(_applicationSettings.SpotifyShareBaseUri) + @"/user/([^/]*)/playlist/([^/?]+)[^/\r\n]*", RegexOptions.Compiled);
        }

        public SpotifyUriTypes GetUriType(string uri)
        {
            if (_trackUriRegex.Match(uri).Success)
            {
                return SpotifyUriTypes.Track;
            }
            else if (_albumUriRegex.Match(uri).Success)
            {
                return SpotifyUriTypes.Album;
            }
            else if (_artistUriRegex.Match(uri).Success)
            {
                return SpotifyUriTypes.Artist;
            }
            else if (_playlistUriRegex.Match(uri).Success)
            {
                return SpotifyUriTypes.Playlist;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(uri), $"Unknown uri type: {uri}");
            }
        }

        private string GetIdFromGenericUri(string uri, Regex regex)
        {
            var match = regex.Match(uri);
            if (match.Success && match.Groups.Count == 2)
            {
                return match.Groups[1].Value;
            }
            else
            {
                throw new Exception($"Match {uri} with {regex} failed");
            }
        }

        public string ParseTrackUri(string trackUri)
        {
            return GetIdFromGenericUri(trackUri, _trackUriRegex);
        }

        public string ParseAlbumUri(string albumUri)
        {
            return GetIdFromGenericUri(albumUri, _albumUriRegex);
        }

        public string ParseArtistUri(string artistUri)
        {
            return GetIdFromGenericUri(artistUri, _artistUriRegex);
        }

        public SpotifyPlaylistUriParts ParsePlaylistUri(string playlistUri)
        {
            var match = _playlistUriRegex.Match(playlistUri);
            if (match.Success && match.Groups.Count == 3)
            {
                return new SpotifyPlaylistUriParts()
                {
                    UserId = match.Groups[1].Value,
                    PlaylistId = match.Groups[2].Value,
                };
            }
            else
            {
                throw new Exception($"Match {playlistUri} with {_playlistUriRegex} failed");
            }
        }
    }
}
