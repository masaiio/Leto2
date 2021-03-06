﻿using Discord.Commands;
using Leto2bot.Extensions;
using System.Linq;
using Discord;
using Leto2bot.Services;
using System.Threading.Tasks;
using Leto2bot.Attributes;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Leto2bot.Services.Database.Models;
using Leto2bot.Services.Permissions;
using Leto2bot.Services.Help;

namespace Leto2bot.Modules.Help
{
    public class Help : Leto2TopLevelModule
    {
        public const string PatreonUrl = "https://patreon.com/leto2bot";
        public const string PaypalUrl = "https://paypal.me/Kwoth";
        private readonly IBotCredentials _creds;
        private readonly BotConfig _config;
        private readonly CommandService _cmds;
        private readonly GlobalPermissionService _perms;
        private readonly HelpService _h;

        public string HelpString => String.Format(_config.HelpString, _creds.ClientId, Prefix);
        public string DMHelpString => _config.DMHelpString;

        public Help(IBotCredentials creds, GlobalPermissionService perms, BotConfig config, CommandService cmds, HelpService h)
        {
            _creds = creds;
            _config = config;
            _cmds = cmds;
            _perms = perms;
            _h = h;
        }

        [Leto2Command, Usage, Description, Aliases]
        public async Task Modules()
        {
            var embed = new EmbedBuilder().WithOkColor()
                .WithFooter(efb => efb.WithText("ℹ️" + GetText("modules_footer", Prefix)))
                .WithTitle(GetText("list_of_modules"))
                .WithDescription(string.Join("\n",
                                     _cmds.Modules.GroupBy(m => m.GetTopLevelModule())
                                         .Where(m => !_perms.BlockedModules.Contains(m.Key.Name.ToLowerInvariant()))
                                         .Select(m => "• " + m.Key.Name)
                                         .OrderBy(s => s)));
            await Context.Channel.EmbedAsync(embed).ConfigureAwait(false);
        }

        [Leto2Command, Usage, Description, Aliases]
        public async Task Commands([Remainder] string module = null)
        {
            var channel = Context.Channel;

            module = module?.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(module))
                return;
            var cmds = _cmds.Commands.Where(c => c.Module.GetTopLevelModule().Name.ToUpperInvariant().StartsWith(module))
                                                .Where(c => !_perms.BlockedCommands.Contains(c.Aliases.First().ToLowerInvariant()))
                                                  .OrderBy(c => c.Aliases.First())
                                                  .Distinct(new CommandTextEqualityComparer())
                                                  .AsEnumerable();

            var cmdsArray = cmds as CommandInfo[] ?? cmds.ToArray();
            if (!cmdsArray.Any())
            {
                await ReplyErrorLocalized("module_not_found").ConfigureAwait(false);
                return;
            }
            var j = 0;
            var groups = cmdsArray.GroupBy(x => j++ / 48).ToArray();

            for (int i = 0; i < groups.Count(); i++)
            {
                await channel.SendTableAsync(i == 0 ? $"📃 **{GetText("list_of_commands")}**\n" : "", groups.ElementAt(i), el => $"{Prefix + el.Aliases.First(),-15} {"[" + el.Aliases.Skip(1).FirstOrDefault() + "]",-8}").ConfigureAwait(false);
            }

            await ConfirmLocalized("commands_instr", Prefix).ConfigureAwait(false);
        }
        [Leto2Command, Usage, Description, Aliases]
        [Priority(1)]
        public async Task H([Remainder] string fail)
        {
            await ReplyErrorLocalized("command_not_found").ConfigureAwait(false);
        }

        [Leto2Command, Usage, Description, Aliases]
        [Priority(0)]
        public async Task H([Remainder] CommandInfo com = null)
        {
            var channel = Context.Channel;

            if (com == null)
            {
                IMessageChannel ch = channel is ITextChannel ? await ((IGuildUser)Context.User).GetOrCreateDMChannelAsync() : channel;
                await ch.SendMessageAsync(HelpString).ConfigureAwait(false);
                return;
            }

            //if (com == null)
            //{
            //    await ReplyErrorLocalized("command_not_found").ConfigureAwait(false);
            //    return;
            //}

            var embed = _h.GetCommandHelp(com, Context.Guild);
            await channel.EmbedAsync(embed).ConfigureAwait(false);
        }

        [Leto2Command, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        [OwnerOnly]
        public async Task Hgit()
        {
            var helpstr = new StringBuilder();
            helpstr.AppendLine(GetText("cmdlist_donate", PatreonUrl, PaypalUrl) + "\n");
            helpstr.AppendLine("##"+ GetText("table_of_contents"));
            helpstr.AppendLine(string.Join("\n", _cmds.Modules.Where(m => m.GetTopLevelModule().Name.ToLowerInvariant() != "help")
                .Select(m => m.GetTopLevelModule().Name)
                .Distinct()
                .OrderBy(m => m)
                .Prepend("Help")
                .Select(m => string.Format("- [{0}](#{1})", m, m.ToLowerInvariant()))));
            helpstr.AppendLine();
            string lastModule = null;
            foreach (var com in _cmds.Commands.OrderBy(com => com.Module.GetTopLevelModule().Name).GroupBy(c => c.Aliases.First()).Select(g => g.First()))
            {
                var module = com.Module.GetTopLevelModule();
                if (module.Name != lastModule)
                {
                    if (lastModule != null)
                    {
                        helpstr.AppendLine();
                        helpstr.AppendLine($"###### [{GetText("back_to_toc")}](#{GetText("table_of_contents").ToLowerInvariant().Replace(' ', '-')})");
                    }
                    helpstr.AppendLine();
                    helpstr.AppendLine("### " + module.Name + "  ");
                    helpstr.AppendLine($"{GetText("cmd_and_alias")} | {GetText("desc")} | {GetText("usage")}");
                    helpstr.AppendLine("----------------|--------------|-------");
                    lastModule = module.Name;
                }
                helpstr.AppendLine($"{string.Join(" ", com.Aliases.Select(a => "`" + Prefix + a + "`"))} |" +
                                   $" {string.Format(com.Summary, Prefix)} {_h.GetCommandRequirements(com, Context.Guild)} |" +
                                   $" {string.Format(com.Remarks, Prefix)}");
            }
            File.WriteAllText("../../docs/Commands List.md", helpstr.ToString());
            await ReplyConfirmLocalized("commandlist_regen").ConfigureAwait(false);
        }

        [Leto2Command, Usage, Description, Aliases]
        public async Task Guide()
        {
            await ConfirmLocalized("guide", 
                "http://leto2bot.readthedocs.io/en/latest/Commands%20List/",
                "http://leto2bot.readthedocs.io/en/latest/").ConfigureAwait(false);
        }

        [Leto2Command, Usage, Description, Aliases]
        public async Task Donate()
        {
            await ReplyConfirmLocalized("donate", PatreonUrl, PaypalUrl).ConfigureAwait(false);
        }
    }

    public class CommandTextEqualityComparer : IEqualityComparer<CommandInfo>
    {
        public bool Equals(CommandInfo x, CommandInfo y) => x.Aliases.First() == y.Aliases.First();

        public int GetHashCode(CommandInfo obj) => obj.Aliases.First().GetHashCode();

    }
}
