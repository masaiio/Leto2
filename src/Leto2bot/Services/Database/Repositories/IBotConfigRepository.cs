using Microsoft.EntityFrameworkCore;
using Leto2bot.Services.Database.Models;
using System;
using System.Linq;

namespace Leto2bot.Services.Database.Repositories
{
    public interface IBotConfigRepository : IRepository<BotConfig>
    {
        BotConfig GetOrCreate(Func<DbSet<BotConfig>, IQueryable<BotConfig>> includes = null);
    }
}
