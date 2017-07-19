﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Leto2bot.Attributes;
using Leto2bot.Extensions;
using Leto2bot.Services.Administration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class TimeZoneCommands : Leto2Submodule
        {
            private readonly GuildTimezoneService _service;

            public TimeZoneCommands(GuildTimezoneService service)
            {
                _service = service;
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Timezones(int page = 1)
            {
                page--;

                if (page < 0 || page > 20)
                    return;

                var timezones = TimeZoneInfo.GetSystemTimeZones()
                    .OrderBy(x => x.BaseUtcOffset)
                    .ToArray();
                var timezonesPerPage = 20;

                await Context.Channel.SendPaginatedConfirmAsync((DiscordSocketClient)Context.Client, page, 
                    (curPage) => new EmbedBuilder()
                        .WithOkColor()
                        .WithTitle(GetText("timezones_available"))
                        .WithDescription(string.Join("\n", timezones.Skip(curPage * timezonesPerPage).Take(timezonesPerPage).Select(x => $"`{x.Id,-25}` {(x.BaseUtcOffset < TimeSpan.Zero? "-" : "+")}{x.BaseUtcOffset:hhmm}"))),
                    timezones.Length / timezonesPerPage);
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Timezone([Remainder] string id = null)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    await ReplyConfirmLocalized("timezone_guild", _service.GetTimeZoneOrUtc(Context.Guild.Id)).ConfigureAwait(false);
                    return;
                }

                TimeZoneInfo tz;
                try { tz = TimeZoneInfo.FindSystemTimeZoneById(id); } catch { tz = null; }

                _service.SetTimeZone(Context.Guild.Id, tz);

                if (tz == null)
                {
                    await ReplyErrorLocalized("timezone_not_found").ConfigureAwait(false);
                    return;
                }

                await Context.Channel.SendConfirmAsync(tz.ToString()).ConfigureAwait(false);
            }
        }
    }
}
