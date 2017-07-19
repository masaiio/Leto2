using Discord;
using Leto2bot.Services.Database.Models;

namespace Leto2bot.Services.Database.Repositories
{
    public interface IDiscordUserRepository : IRepository<DiscordUser>
    {
        DiscordUser GetOrCreate(IUser original);
    }
}
