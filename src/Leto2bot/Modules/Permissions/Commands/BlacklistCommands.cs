using Discord;
using Discord.Commands;
using Leto2bot.Attributes;
using Leto2bot.Modules.Games.Trivia;
using Leto2bot.Services;
using Leto2bot.Services.Database.Models;
using Leto2bot.Services.Permissions;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Permissions
{
    public partial class Permissions
    {
        public enum AddRemove
        {
            Add,
            Rem
        }

        [Group]
        public class BlacklistCommands : Leto2Submodule
        {
            private readonly BlacklistService _bs;
            private readonly DbService _db;
            private readonly IBotCredentials _creds;

            private ConcurrentHashSet<ulong> BlacklistedUsers => _bs.BlacklistedUsers;
            private ConcurrentHashSet<ulong> BlacklistedGuilds => _bs.BlacklistedGuilds;
            private ConcurrentHashSet<ulong> BlacklistedChannels => _bs.BlacklistedChannels;

            public BlacklistCommands(BlacklistService bs, DbService db, IBotCredentials creds)
            {
                _bs = bs;
                _db = db;
                _creds = creds;
            }

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public Task UserBlacklist(AddRemove action, ulong id)
                => Blacklist(action, id, BlacklistType.User);

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public Task UserBlacklist(AddRemove action, IUser usr)
                => Blacklist(action, usr.Id, BlacklistType.User);

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public Task ChannelBlacklist(AddRemove action, ulong id)
                => Blacklist(action, id, BlacklistType.Channel);

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public Task ServerBlacklist(AddRemove action, ulong id)
                => Blacklist(action, id, BlacklistType.Server);

            [Leto2Command, Usage, Description, Aliases]
            [OwnerOnly]
            public Task ServerBlacklist(AddRemove action, IGuild guild)
                => Blacklist(action, guild.Id, BlacklistType.Server);

            private async Task Blacklist(AddRemove action, ulong id, BlacklistType type)
            {
                if(action == AddRemove.Add && _creds.OwnerIds.Contains(id))
                    return;

                using (var uow = _db.UnitOfWork)
                {
                    if (action == AddRemove.Add)
                    {
                        var item = new BlacklistItem { ItemId = id, Type = type };
                        uow.BotConfig.GetOrCreate().Blacklist.Add(item);
                        if (type == BlacklistType.Server)
                        {
                            BlacklistedGuilds.Add(id);
                        }
                        else if (type == BlacklistType.Channel)
                        {
                            BlacklistedChannels.Add(id);
                        }
                        else if (type == BlacklistType.User)
                        {
                            BlacklistedUsers.Add(id);
                        }                        
                    }
                    else
                    {
                        uow.BotConfig.GetOrCreate().Blacklist.RemoveWhere(bi => bi.ItemId == id && bi.Type == type);
                        if (type == BlacklistType.Server)
                        {
                            BlacklistedGuilds.TryRemove(id);
                        }
                        else if (type == BlacklistType.Channel)
                        {
                            BlacklistedChannels.TryRemove(id);
                        }
                        else if (type == BlacklistType.User)
                        {
                            BlacklistedUsers.TryRemove(id);
                        }
                    }
                    await uow.CompleteAsync().ConfigureAwait(false);
                }
                if (action == AddRemove.Add)
                {
                    TriviaGame tg;
                    switch (type)
                    {
                        case BlacklistType.Server:
                            Games.Games.TriviaCommands.RunningTrivias.TryRemove(id, out tg);
                            if (tg != null)
                            {
                                await tg.StopGame().ConfigureAwait(false);
                            }
                            break;
                        case BlacklistType.Channel:
                            var item = Games.Games.TriviaCommands.RunningTrivias.FirstOrDefault(kvp => kvp.Value.Channel.Id == id);
                            Games.Games.TriviaCommands.RunningTrivias.TryRemove(item.Key, out tg);
                            if (tg != null)
                            {
                                await tg.StopGame().ConfigureAwait(false);
                            }
                            break;
                        case BlacklistType.User:
                            break;
                    }

                }

                if(action == AddRemove.Add)
                    await ReplyConfirmLocalized("blacklisted", Format.Code(type.ToString()), Format.Code(id.ToString())).ConfigureAwait(false);
                else
                    await ReplyConfirmLocalized("unblacklisted", Format.Code(type.ToString()), Format.Code(id.ToString())).ConfigureAwait(false);
            }
        }
    }
}
