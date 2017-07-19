using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Leto2bot.DataStructures
{
    public class NoPublicBot : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
#if GLOBAL_NADEKo
            return Task.FromResult(PreconditionResult.FromError("Not available on the public bot"));
#else
            return Task.FromResult(PreconditionResult.FromSuccess());
#endif
        }
    }
}
