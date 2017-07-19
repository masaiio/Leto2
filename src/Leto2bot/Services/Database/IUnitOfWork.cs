using Leto2bot.Services.Database.Repositories;
using System;
using System.Threading.Tasks;

namespace Leto2bot.Services.Database
{
    public interface IUnitOfWork : IDisposable
    {
        Leto2Context _context { get; }

        IQuoteRepository Quotes { get; }
        IGuildConfigRepository GuildConfigs { get; }
        IDonatorsRepository Donators { get; }
        IReminderRepository Reminders { get; }
        ISelfAssignedRolesRepository SelfAssignedRoles { get; }
        IBotConfigRepository BotConfig { get; }
        IUnitConverterRepository ConverterUnits { get; }
        ICustomReactionRepository CustomReactions { get; }
        ICurrencyRepository Currency { get; }
        ICurrencyTransactionsRepository CurrencyTransactions { get; }
        IMusicPlaylistRepository MusicPlaylists { get; }
        IWaifuRepository Waifus { get; }
        IDiscordUserRepository DiscordUsers { get; }
        IWarningsRepository Warnings { get; }

        int Complete();
        Task<int> CompleteAsync();
    }
}
