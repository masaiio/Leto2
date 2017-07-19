using Discord.Commands;
using Leto2bot.Services;
using System.Runtime.CompilerServices;

namespace Leto2bot.Attributes
{
    public class Leto2Command : CommandAttribute
    {
        public Leto2Command([CallerMemberName] string memberName="") : base(Localization.LoadCommandString(memberName.ToLowerInvariant() + "_cmd").Split(' ')[0])
        {

        }
    }
}
