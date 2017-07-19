﻿using System.Threading.Tasks;
using Discord.Commands;
using Leto2bot.Attributes;
using System;
using Leto2bot.Services;
using Leto2bot.Services.Database.Models;
using Leto2bot.Extensions;
using Discord;
using Leto2bot.Services.Utility;

namespace Leto2bot.Modules.Utility
{
    public partial class Utility
    {
        [Group]
        public class PatreonCommands : Leto2Submodule
        {
            private readonly PatreonRewardsService _patreon;
            private readonly IBotCredentials _creds;
            private readonly BotConfig _config;
            private readonly DbService _db;
            private readonly CurrencyService _currency;

            public PatreonCommands(PatreonRewardsService p, IBotCredentials creds, BotConfig config, DbService db, CurrencyService currency)
            {
                _creds = creds;
                _config = config;
                _db = db;
                _currency = currency;
                _patreon = p;                
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            [RequireContext(ContextType.DM)]
            public async Task PatreonRewardsReload()
            {
                if (string.IsNullOrWhiteSpace(_creds.PatreonAccessToken))
                    return;
                await _patreon.RefreshPledges(true).ConfigureAwait(false);

                await Context.Channel.SendConfirmAsync("👌").ConfigureAwait(false);
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.DM)]
            public async Task ClaimPatreonRewards()
            {
                if (string.IsNullOrWhiteSpace(_creds.PatreonAccessToken))
                    return;

                if (DateTime.UtcNow.Day < 5)
                {
                    await ReplyErrorLocalized("clpa_too_early").ConfigureAwait(false);
                    return;
                }
                int amount = 0;
                try
                {
                    amount = await _patreon.ClaimReward(Context.User.Id).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _log.Warn(ex);
                }

                if (amount > 0)
                {
                    await ReplyConfirmLocalized("clpa_success", amount + _config.CurrencySign).ConfigureAwait(false);
                    return;
                }
                var rem = (_patreon.Interval - (DateTime.UtcNow - _patreon.LastUpdate));
                var helpcmd = Format.Code(Prefix + "donate");
                await Context.Channel.EmbedAsync(new EmbedBuilder().WithOkColor()
                    .WithDescription(GetText("clpa_fail"))
                    .AddField(efb => efb.WithName(GetText("clpa_fail_already_title")).WithValue(GetText("clpa_fail_already")))
                    .AddField(efb => efb.WithName(GetText("clpa_fail_wait_title")).WithValue(GetText("clpa_fail_wait")))
                    .AddField(efb => efb.WithName(GetText("clpa_fail_conn_title")).WithValue(GetText("clpa_fail_conn")))
                    .AddField(efb => efb.WithName(GetText("clpa_fail_sup_title")).WithValue(GetText("clpa_fail_sup", helpcmd)))
                    .WithFooter(efb => efb.WithText(GetText("clpa_next_update", rem))))
                    .ConfigureAwait(false);
            }
        }

    }
}