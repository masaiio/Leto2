using Discord;
using Discord.Commands;
using Leto2bot.Attributes;
using Leto2bot.Extensions;
using Leto2bot.Services;
using Leto2bot.Services.Administration;
using System.Linq;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class AutoAssignRoleCommands : Leto2Submodule
        {
            private readonly DbService _db;
            private readonly AutoAssignRoleService _service;

            public AutoAssignRoleCommands(AutoAssignRoleService service, DbService db)
            {
                _db = db;
                _service = service;
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageRoles)]
            public async Task AutoAssignRole([Remainder] IRole role = null)
            {
                var guser = (IGuildUser)Context.User;
                if (role != null)
                    if (Context.User.Id != guser.Guild.OwnerId && guser.GetRoles().Max(x => x.Position) <= role.Position)
                        return;

                using (var uow = _db.UnitOfWork)
                {
                    var conf = uow.GuildConfigs.For(Context.Guild.Id, set => set);
                    if (role == null)
                    {
                        conf.AutoAssignRoleId = 0;
                        _service.AutoAssignedRoles.TryRemove(Context.Guild.Id, out ulong throwaway);
                    }
                    else
                    {
                        conf.AutoAssignRoleId = role.Id;
                        _service.AutoAssignedRoles.AddOrUpdate(Context.Guild.Id, role.Id, (key, val) => role.Id);
                    }

                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                if (role == null)
                {
                    await ReplyConfirmLocalized("aar_disabled").ConfigureAwait(false);
                    return;
                }

                await ReplyConfirmLocalized("aar_enabled").ConfigureAwait(false);
            }
        }
    }
}
