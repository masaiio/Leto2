using Discord;

namespace Leto2bot.Extensions
{
    public static class MusicExtensions
    {
        public static EmbedAuthorBuilder WithMusicIcon(this EmbedAuthorBuilder eab) =>
            eab.WithIconUrl("http://i.imgur.com/nhKS3PT.png");
    }
}
