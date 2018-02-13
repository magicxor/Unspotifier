using System.Collections.Generic;
using FluentSpotifyApi.Model;
using Unspotifier.Core.Models;

namespace Unspotifier.Core.Services
{
    public class TelegraphPublisher
    {
        private readonly ApplicationSettings _applicationSettings;

        public TelegraphPublisher(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public void PublishPlaylist(IEnumerable<PlaylistTrack> playlistTracks)
        {
            // todo: implement
        }
    }
}