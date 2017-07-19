using Leto2bot.Services.Database.Repositories;
using Leto2bot.Services.Database.Repositories.Impl;
using System;
using System.Threading.Tasks;

namespace Leto2bot.Services.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        public Leto2Context _context { get; }

        private IQuoteRepository _quotes;
        public IQuoteRepository Quotes => _quotes ?? (_quotes = new QuoteRepository(_context));

        private IGuildConfigRepository _guildConfigs;
        public IGuildConfigRepository GuildConfigs => _guildConfigs ?? (_guildConfigs = new GuildConfigRepository(_context));

        private IDonatorsRepository _donators;
        public IDonatorsRepository Donators => _donators ?? (_donators = new DonatorsRepository(_context));

        private IReminderRepository _reminders;
        public IReminderRepository Reminders => _reminders ?? (_reminders = new ReminderRepository(_context));

        private ISelfAssignedRolesRepository _selfAssignedRoles;
        public ISelfAssignedRolesRepository SelfAssignedRoles => _selfAssignedRoles ?? (_selfAssignedRoles = new SelfAssignedRolesRepository(_context));

        private IBotConfigRepository _botConfig;
        public IBotConfigRepository BotConfig => _botConfig ?? (_botConfig = new BotConfigRepository(_context));

        private ICurrencyRepository _currency;
        public ICurrencyRepository Currency => _currency ?? (_currency = new CurrencyRepository(_context));

        private ICurrencyTransactionsRepository _currencyTransactions;
        public ICurrencyTransactionsRepository CurrencyTransactions => _currencyTransactions ?? (_currencyTransactions = new CurrencyTransactionsRepository(_context));

        private IUnitConverterRepository _conUnits;
        public IUnitConverterRepository ConverterUnits => _conUnits ?? (_conUnits = new UnitConverterRepository(_context));

        private IMusicPlaylistRepository _musicPlaylists;
        public IMusicPlaylistRepository MusicPlaylists => _musicPlaylists ?? (_musicPlaylists = new MusicPlaylistRepository(_context));

        private ICustomReactionRepository _customReactions;
        public ICustomReactionRepository CustomReactions => _customReactions ?? (_customReactions = new CustomReactionsRepository(_context));

        private IWaifuRepository _waifus;
        public IWaifuRepository Waifus => _waifus ?? (_waifus = new WaifuRepository(_context));

        private IDiscordUserRepository _discordUsers;
        public IDiscordUserRepository DiscordUsers => _discordUsers ?? (_discordUsers = new DiscordUserRepository(_context));

        private IWarningsRepository _warnings;
        public IWarningsRepository Warnings => _warnings ?? (_warnings = new WarningsRepository(_context));

        public UnitOfWork(Leto2Context context)
        {
            _context = context;
        }

        public int Complete() =>
            _context.SaveChanges();

        public Task<int> CompleteAsync() => 
            _context.SaveChangesAsync();

        private bool disposed = false;

        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
                if (disposing)
                    _context.Dispose();
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
