using Discord.Commands;
using Leto2bot.Services;
using System.Runtime.CompilerServices;

namespace Leto2bot.Attributes
{
    public class Usage : RemarksAttribute
    {
        public Usage([CallerMemberName] string memberName="") : base(Localization.LoadCommandString(memberName.ToLowerInvariant()+"_usage"))
        {

        }
    }
}
