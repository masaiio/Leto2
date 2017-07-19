﻿using Discord;
using Discord.Commands;
using Leto2bot.Attributes;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class PrefixCommands : Leto2Submodule
        {
            [Leto2Command, Usage, Description, Aliases]
            [Priority(0)]
            public new async Task Prefix()
            {
                await ReplyConfirmLocalized("prefix_current", Format.Code(_cmdHandler.GetPrefix(Context.Guild))).ConfigureAwait(false);
                return;
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.Administrator)]
            [Priority(1)]
            public new async Task Prefix([Remainder]string prefix)
            {
                if (string.IsNullOrWhiteSpace(prefix))
                    return;

                var oldPrefix = base.Prefix;
                var newPrefix = _cmdHandler.SetPrefix(Context.Guild, prefix);

                await ReplyConfirmLocalized("prefix_new", Format.Code(oldPrefix), Format.Code(newPrefix)).ConfigureAwait(false);
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task DefPrefix([Remainder]string prefix)
            {
                if (string.IsNullOrWhiteSpace(prefix))
                {
                    await ReplyConfirmLocalized("defprefix_current", _cmdHandler.DefaultPrefix).ConfigureAwait(false);
                    return;
                }

                var oldPrefix = _cmdHandler.DefaultPrefix;
                var newPrefix = _cmdHandler.SetDefaultPrefix(prefix);

                await ReplyConfirmLocalized("defprefix_new", Format.Code(oldPrefix), Format.Code(newPrefix)).ConfigureAwait(false);
            }
        }
    }
}
