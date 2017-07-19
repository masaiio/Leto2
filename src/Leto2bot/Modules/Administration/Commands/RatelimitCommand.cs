using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Leto2bot.Attributes;
using Leto2bot.Extensions;
using Leto2bot.Services;
using Leto2bot.Services.Administration;
using Leto2bot.Services.Database.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class SlowModeCommands : Leto2Submodule
        {
            private readonly SlowmodeService _service;
            private readonly DbService _db;

            public SlowModeCommands(SlowmodeService service, DbService db)
            {
                _service = service;
                _db = db;
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task Slowmode()
            {
                if (_service.RatelimitingChannels.TryRemove(Context.Channel.Id, out Ratelimiter throwaway))
                {
                    throwaway.CancelSource.Cancel();
                    await ReplyConfirmLocalized("slowmode_disabled").ConfigureAwait(false);
                }
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task Slowmode(int msg, int perSec)
            {
                await Slowmode().ConfigureAwait(false); // disable if exists
                
                if (msg < 1 || perSec < 1 || msg > 100 || perSec > 3600)
                {
                    await ReplyErrorLocalized("invalid_params").ConfigureAwait(false);
                    return;
                }
                var toAdd = new Ratelimiter(_service)
                {
                    ChannelId = Context.Channel.Id,
                    MaxMessages = msg,
                    PerSeconds = perSec,
                };
                if(_service.RatelimitingChannels.TryAdd(Context.Channel.Id, toAdd))
                {
                    await Context.Channel.SendConfirmAsync(GetText("slowmode_init"),
                            GetText("slowmode_desc", Format.Bold(toAdd.MaxMessages.ToString()), Format.Bold(toAdd.PerSeconds.ToString())))
                                                .ConfigureAwait(false);
                }
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            [Priority(1)]
            public async Task SlowmodeWhitelist(IGuildUser user)
            {
                var siu = new SlowmodeIgnoredUser
                {
                    UserId = user.Id
                };

                HashSet<SlowmodeIgnoredUser> usrs;
                bool removed;
                using (var uow = _db.UnitOfWork)
                {
                    usrs = uow.GuildConfigs.For(Context.Guild.Id, set => set.Include(x => x.SlowmodeIgnoredUsers))
                        .SlowmodeIgnoredUsers;

                    if (!(removed = usrs.Remove(siu)))
                        usrs.Add(siu);

                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                _service.IgnoredUsers.AddOrUpdate(Context.Guild.Id, new HashSet<ulong>(usrs.Select(x => x.UserId)), (key, old) => new HashSet<ulong>(usrs.Select(x => x.UserId)));

                if(removed)
                    await ReplyConfirmLocalized("slowmodewl_user_stop", Format.Bold(user.ToString())).ConfigureAwait(false);
                else
                    await ReplyConfirmLocalized("slowmodewl_user_start", Format.Bold(user.ToString())).ConfigureAwait(false);
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            [Priority(0)]
            public async Task SlowmodeWhitelist(IRole role)
            {
                var sir = new SlowmodeIgnoredRole
                {
                    RoleId = role.Id
                };

                HashSet<SlowmodeIgnoredRole> roles;
                bool removed;
                using (var uow = _db.UnitOfWork)
                {
                    roles = uow.GuildConfigs.For(Context.Guild.Id, set => set.Include(x => x.SlowmodeIgnoredRoles))
                        .SlowmodeIgnoredRoles;

                    if (!(removed = roles.Remove(sir)))
                        roles.Add(sir);

                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                _service.IgnoredRoles.AddOrUpdate(Context.Guild.Id, new HashSet<ulong>(roles.Select(x => x.RoleId)), (key, old) => new HashSet<ulong>(roles.Select(x => x.RoleId)));

                if (removed)
                    await ReplyConfirmLocalized("slowmodewl_role_stop", Format.Bold(role.ToString())).ConfigureAwait(false);
                else
                    await ReplyConfirmLocalized("slowmodewl_role_start", Format.Bold(role.ToString())).ConfigureAwait(false);
            }
        }
    }
}