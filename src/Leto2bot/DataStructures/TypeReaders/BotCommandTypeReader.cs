﻿using Discord.Commands;
using Leto2bot.Services;
using Leto2bot.Services.CustomReactions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Leto2bot.TypeReaders
{
    public class CommandTypeReader : TypeReader
    {
        private readonly CommandService _cmds;
        private readonly CommandHandler _cmdHandler;

        public CommandTypeReader(CommandService cmds, CommandHandler cmdHandler)
        {
            _cmds = cmds;
            _cmdHandler = cmdHandler;
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider _)
        {
            input = input.ToUpperInvariant();
            var prefix = _cmdHandler.GetPrefix(context.Guild);
            if (!input.StartsWith(prefix.ToUpperInvariant()))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "No such command found."));

            input = input.Substring(prefix.Length);

            var cmd = _cmds.Commands.FirstOrDefault(c => 
                c.Aliases.Select(a => a.ToUpperInvariant()).Contains(input));
            if (cmd == null)
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "No such command found."));

            return Task.FromResult(TypeReaderResult.FromSuccess(cmd));
        }
    }

    public class CommandOrCrTypeReader : CommandTypeReader
    {
        private readonly CustomReactionsService _crs;

        public CommandOrCrTypeReader(CustomReactionsService crs, CommandService cmds, CommandHandler cmdHandler) : base(cmds, cmdHandler)
        {
            _crs = crs;
        }

        public override async Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider _)
        {
            input = input.ToUpperInvariant();

            if (_crs.GlobalReactions.Any(x => x.Trigger.ToUpperInvariant() == input))
            {
                return TypeReaderResult.FromSuccess(new CommandOrCrInfo(input));
            }
            var guild = context.Guild;
            if (guild != null)
            {
                if (_crs.GuildReactions.TryGetValue(guild.Id, out var crs))
                {
                    if (crs.Concat(_crs.GlobalReactions).Any(x => x.Trigger.ToUpperInvariant() == input))
                    {
                        return TypeReaderResult.FromSuccess(new CommandOrCrInfo(input));
                    }
                }
            }

            var cmd = await base.Read(context, input, _);
            if (cmd.IsSuccess)
            {
                return TypeReaderResult.FromSuccess(new CommandOrCrInfo(((CommandInfo)cmd.Values.First().Value).Name));
            }
            return TypeReaderResult.FromError(CommandError.ParseFailed, "No such command or cr found.");
        }
    }

    public class CommandOrCrInfo
    {
        public string Name { get; set; }

        public CommandOrCrInfo(string input)
        {
            this.Name = input;
        }
    }
}
