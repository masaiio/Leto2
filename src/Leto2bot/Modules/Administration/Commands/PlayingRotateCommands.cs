﻿using Discord.Commands;
using Leto2bot.Attributes;
using Leto2bot.Services;
using Leto2bot.Services.Administration;
using Leto2bot.Services.Database.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class PlayingRotateCommands : Leto2Submodule
        {
            private static readonly object _locker = new object();
            private readonly DbService _db;
            private readonly PlayingRotateService _service;

            public PlayingRotateCommands(PlayingRotateService service, DbService db)
            {
                _db = db;
                _service = service;
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task RotatePlaying()
            {
                bool enabled;
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.BotConfig.GetOrCreate();

                    enabled = config.RotatingStatuses = !config.RotatingStatuses;
                    uow.Complete();
                }
                if (enabled)
                    await ReplyConfirmLocalized("ropl_enabled").ConfigureAwait(false);
                else
                    await ReplyConfirmLocalized("ropl_disabled").ConfigureAwait(false);
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task AddPlaying([Remainder] string status)
            {
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.BotConfig.GetOrCreate();
                    var toAdd = new PlayingStatus { Status = status };
                    config.RotatingStatusMessages.Add(toAdd);
                    await uow.CompleteAsync();
                }

                await ReplyConfirmLocalized("ropl_added").ConfigureAwait(false);
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task ListPlaying()
            {
                if (!_service.BotConfig.RotatingStatusMessages.Any())
                    await ReplyErrorLocalized("ropl_not_set").ConfigureAwait(false);
                else
                {
                    var i = 1;
                    await ReplyConfirmLocalized("ropl_list",
                            string.Join("\n\t", _service.BotConfig.RotatingStatusMessages.Select(rs => $"`{i++}.` {rs.Status}")))
                        .ConfigureAwait(false);
                }

            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task RemovePlaying(int index)
            {
                index -= 1;

                string msg;
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.BotConfig.GetOrCreate();

                    if (index >= config.RotatingStatusMessages.Count)
                        return;
                    msg = config.RotatingStatusMessages[index].Status;
                    config.RotatingStatusMessages.RemoveAt(index);
                    await uow.CompleteAsync();
                }
                await ReplyConfirmLocalized("reprm", msg).ConfigureAwait(false);
            }
        }
    }
}