using Discord;
using Discord.Commands;
using Leto2bot.Attributes;
using Leto2bot.Services;
using Leto2bot.Services.Database.Models;
using Leto2bot.Services.Permissions;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Permissions
{
    public partial class Permissions
    {
        [Group]
        public class ResetPermissionsCommands : Leto2Submodule
        {
            private readonly PermissionService _service;
            private readonly DbService _db;
            private readonly GlobalPermissionService _globalPerms;

            public ResetPermissionsCommands(PermissionService service, GlobalPermissionService globalPerms, DbService db)
            {
                _service = service;
                _db = db;
                _globalPerms = globalPerms;
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.Administrator)]
            public async Task ResetPermissions()
            {
                //todo 50 move to service
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.GuildConfigs.GcWithPermissionsv2For(Context.Guild.Id);
                    config.Permissions = Permissionv2.GetDefaultPermlist;
                    await uow.CompleteAsync();
                    _service.UpdateCache(config);
                }
                await ReplyConfirmLocalized("perms_reset").ConfigureAwait(false);
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task ResetGlobalPermissions()
            {
                //todo 50 move to service
                using (var uow = _db.UnitOfWork)
                {
                    var gc = uow.BotConfig.GetOrCreate();
                    gc.BlockedCommands.Clear();
                    gc.BlockedModules.Clear();

                    _globalPerms.BlockedCommands.Clear();
                    _globalPerms.BlockedModules.Clear();
                    await uow.CompleteAsync();
                }
                await ReplyConfirmLocalized("global_perms_reset").ConfigureAwait(false);
            }
        }
    }
}
