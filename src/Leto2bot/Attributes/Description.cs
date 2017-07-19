using Discord.Commands;
using Leto2bot.Services;
using System.Runtime.CompilerServices;

namespace Leto2bot.Attributes
{
    public class Description : SummaryAttribute
    {
        public Description([CallerMemberName] string memberName="") : base(Localization.LoadCommandString(memberName.ToLowerInvariant() + "_desc"))
        {

        }
    }
}
