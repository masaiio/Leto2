using Leto2bot.Services.Database.Models;
using System.Collections.Generic;

namespace Leto2bot.Services.Database.Repositories
{
    public interface IMusicPlaylistRepository : IRepository<MusicPlaylist>
    {
        List<MusicPlaylist> GetPlaylistsOnPage(int num);
        MusicPlaylist GetWithSongs(int id);
    }
}
