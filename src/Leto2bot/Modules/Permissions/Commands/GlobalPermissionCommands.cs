﻿using Discord;
using Discord.Commands;
using Leto2bot.Attributes;
using Leto2bot.Extensions;
using Leto2bot.Services;
using Leto2bot.Services.Permissions;
using Leto2bot.TypeReaders;
using System.Linq;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Permissions
{
    public partial class Permissions
    {
        [Group]
        public class GlobalPermissionCommands : Leto2Submodule
        {
            private GlobalPermissionService _service;
            private readonly DbService _db;

            public GlobalPermissionCommands(GlobalPermissionService service, DbService db)
            {
                _service = service;
                _db = db;
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task Lgp()
            {
                if (!_service.BlockedModules.Any() && !_service.BlockedCommands.Any())
                {
                    await ReplyErrorLocalized("lgp_none").ConfigureAwait(false);
                    return;
                }

                var embed = new EmbedBuilder().WithOkColor();

                if (_service.BlockedModules.Any())
                    embed.AddField(efb => efb.WithName(GetText("blocked_modules")).WithValue(string.Join("\n", _service.BlockedModules)).WithIsInline(false));

                if (_service.BlockedCommands.Any())
                    embed.AddField(efb => efb.WithName(GetText("blocked_commands")).WithValue(string.Join("\n", _service.BlockedCommands)).WithIsInline(false));

                await Context.Channel.EmbedAsync(embed).ConfigureAwait(false);
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task Gmod(ModuleOrCrInfo module)
            {
                var moduleName = module.Name.ToLowerInvariant();
                if (_service.BlockedModules.Add(moduleName))
                {
                    using (var uow = _db.UnitOfWork)
                    {
                        var bc = uow.BotConfig.GetOrCreate();
                        bc.BlockedModules.Add(new Services.Database.Models.BlockedCmdOrMdl
                        {
                            Name = moduleName,
                        });
                        uow.Complete();
                    }
                    await ReplyConfirmLocalized("gmod_add", Format.Bold(module.Name)).ConfigureAwait(false);
                    return;
                }
                else if (_service.BlockedModules.TryRemove(moduleName))
                {
                    using (var uow = _db.UnitOfWork)
                    {
                        var bc = uow.BotConfig.GetOrCreate();
                        bc.BlockedModules.RemoveWhere(x => x.Name == moduleName);
                        uow.Complete();
                    }
                    await ReplyConfirmLocalized("gmod_remove", Format.Bold(module.Name)).ConfigureAwait(false);
                    return;
                }
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task Gcmd(CommandOrCrInfo cmd)
            {
                var commandName = cmd.Name.ToLowerInvariant();
                if (_service.BlockedCommands.Add(commandName))
                {
                    using (var uow = _db.UnitOfWork)
                    {
                        var bc = uow.BotConfig.GetOrCreate();
                        bc.BlockedCommands.Add(new Services.Database.Models.BlockedCmdOrMdl
                        {
                            Name = commandName,
                        });
                        uow.Complete();
                    }
                    await ReplyConfirmLocalized("gcmd_add", Format.Bold(cmd.Name)).ConfigureAwait(false);
                    return;
                }
                else if (_service.BlockedCommands.TryRemove(commandName))
                {
                    using (var uow = _db.UnitOfWork)
                    {
                        var bc = uow.BotConfig.GetOrCreate();
                        bc.BlockedCommands.RemoveWhere(x => x.Name == commandName);
                        uow.Complete();
                    }
                    await ReplyConfirmLocalized("gcmd_remove", Format.Bold(cmd.Name)).ConfigureAwait(false);
                    return;
                }
            }
        }
    }
}
