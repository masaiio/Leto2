using System.Threading.Tasks;

namespace Leto2bot.Services.Music.SongResolver.Strategies
{
    public interface IResolveStrategy
    {
        Task<SongInfo> ResolveSong(string query);
    }
}
