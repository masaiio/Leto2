using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Leto2bot.DataStructures.ModuleBehaviors
{
    public interface ILateBlocker
    {
        Task<bool> TryBlockLate(DiscordSocketClient client, IUserMessage msg, IGuild guild, 
            IMessageChannel channel, IUser user, string moduleName, string commandName);
    }
}
