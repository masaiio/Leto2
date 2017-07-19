using Leto2bot.Services.Database.Models;
using Leto2bot.Services.Music.SongResolver.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leto2bot.Services.Music.SongResolver
{
    public interface ISongResolverFactory
    {
        Task<IResolveStrategy> GetResolveStrategy(string query, MusicType? musicType);
    }
}
