using Leto2bot.Services.Database.Models;
using Leto2bot.Services.Impl;
using System;
using System.Threading.Tasks;

namespace Leto2bot.Services.Music.Extensions
{
    public static class Extensions
    {
        public static Task<SongInfo> GetSongInfo(this SoundCloudVideo svideo) =>
            Task.FromResult(new SongInfo
            {
                Title = svideo.FullName,
                Provider = "SoundCloud",
                Uri = () => svideo.StreamLink(),
                ProviderType = MusicType.Soundcloud,
                Query = svideo.TrackLink,
                Thumbnail = svideo.artwork_url,
                TotalTime = TimeSpan.FromMilliseconds(svideo.Duration)
            });
    }
}
