﻿using System;
using System.Threading.Tasks;
using Discord;
using Leto2bot.Extensions;
using Leto2bot.Services.Database.Models;
using Leto2bot.Services.Database;

namespace Leto2bot.Services
{
    public class CurrencyService
    {
        private readonly BotConfig _config;
        private readonly DbService _db;

        public CurrencyService(BotConfig config, DbService db)
        {
            _config = config;
            _db = db;
        }

        public async Task<bool> RemoveAsync(IUser author, string reason, long amount, bool sendMessage)
        {
            var success = await RemoveAsync(author.Id, reason, amount);

            if (success && sendMessage)
                try { await author.SendErrorAsync($"`You lost:` {amount} {_config.CurrencySign}\n`Reason:` {reason}").ConfigureAwait(false); } catch { }

            return success;
        }

        public async Task<bool> RemoveAsync(ulong authorId, string reason, long amount, IUnitOfWork uow = null)
        {
            if (amount < 0)
                throw new ArgumentNullException(nameof(amount));
            
            if (uow == null)
            {
                using (uow = _db.UnitOfWork)
                {
                    var toReturn = InternalRemoveCurrency(authorId, reason, amount, uow);
                    await uow.CompleteAsync().ConfigureAwait(false);
                    return toReturn;
                }
            }

            return InternalRemoveCurrency(authorId, reason, amount, uow);
        }

        private bool InternalRemoveCurrency(ulong authorId, string reason, long amount, IUnitOfWork uow)
        {
            var success = uow.Currency.TryUpdateState(authorId, -amount);
            if (!success)
                return false;
            uow.CurrencyTransactions.Add(new CurrencyTransaction()
            {
                UserId = authorId,
                Reason = reason,
                Amount = -amount,
            });
            return true;
        }

        public async Task AddAsync(IUser author, string reason, long amount, bool sendMessage)
        {
            await AddAsync(author.Id, reason, amount);

            if (sendMessage)
                try { await author.SendConfirmAsync($"`You received:` {amount} {_config.CurrencySign}\n`Reason:` {reason}").ConfigureAwait(false); } catch { }
        }

        public async Task AddAsync(ulong receiverId, string reason, long amount, IUnitOfWork uow = null)
        {
            if (amount < 0)
                throw new ArgumentNullException(nameof(amount));

            var transaction = new CurrencyTransaction()
            {
                UserId = receiverId,
                Reason = reason,
                Amount = amount,
            };

            if (uow == null)
                using (uow = _db.UnitOfWork)
                {
                    uow.Currency.TryUpdateState(receiverId, amount);
                    uow.CurrencyTransactions.Add(transaction);
                    await uow.CompleteAsync();
                }
            else
            {
                uow.Currency.TryUpdateState(receiverId, amount);
                uow.CurrencyTransactions.Add(transaction);
            }
        }
    }
}
