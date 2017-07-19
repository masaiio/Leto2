using Discord.Commands;
using Leto2bot.Services;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Leto2bot.Attributes
{
    public class Aliases : AliasAttribute
    {
        public Aliases([CallerMemberName] string memberName = "") : base(Localization.LoadCommandString(memberName.ToLowerInvariant() + "_cmd").Split(' ').Skip(1).ToArray())
        {
        }
    }
}
