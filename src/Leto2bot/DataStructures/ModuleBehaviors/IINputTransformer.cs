using Discord;
using System.Threading.Tasks;

namespace Leto2bot.DataStructures.ModuleBehaviors
{
    public interface IInputTransformer
    {
        Task<string> TransformInput(IGuild guild, IMessageChannel channel, IUser user, string input);
    }
}
