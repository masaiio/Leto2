using Discord.Commands;
using Leto2bot.Attributes;
using Leto2bot.Services.Utility;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Utility
{
    public partial class Utility
    {
        [Group]
        public class VerboseCommandErrors : Leto2Submodule
        {
            private readonly VerboseErrorsService _ves;

            public VerboseCommandErrors(VerboseErrorsService ves)
            {
                _ves = ves;
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
            public async Task VerboseError()
            {
                var state = _ves.ToggleVerboseErrors(Context.Guild.Id);

                if (state)
                    await ReplyConfirmLocalized("verbose_errors_enabled").ConfigureAwait(false);
                else
                    await ReplyConfirmLocalized("verbose_errors_disabled").ConfigureAwait(false);
            }
        }
    }
}
