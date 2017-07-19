using Discord.Commands;
using Leto2bot.Attributes;
using Leto2bot.Extensions;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Leto2bot.Modules.Games.Hangman;
using Discord;
using Discord.WebSocket;

namespace Leto2bot.Modules.Games
{
    public partial class Games
    {
        [Group]
        public class HangmanCommands : Leto2Submodule
        {
            private readonly DiscordSocketClient _client;

            public HangmanCommands(DiscordSocketClient client)
            {
                _client = client;
            }

            //channelId, game
            public static ConcurrentDictionary<ulong, HangmanGame> HangmanGames { get; } = new ConcurrentDictionary<ulong, HangmanGame>();
            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Hangmanlist()
            {
                await Context.Channel.SendConfirmAsync(Format.Code(GetText("hangman_types", Prefix)) + "\n" + string.Join(", ", HangmanTermPool.data.Keys));
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Hangman([Remainder]string type = "All")
            {
                var hm = new HangmanGame(_client, Context.Channel, type);

                if (!HangmanGames.TryAdd(Context.Channel.Id, hm))
                {
                    await ReplyErrorLocalized("hangman_running").ConfigureAwait(false);
                    return;
                }

                hm.OnEnded += g =>
                {
                    HangmanGames.TryRemove(g.GameChannel.Id, out HangmanGame throwaway);
                };
                try
                {
                    hm.Start();
                }
                catch (Exception ex)
                {
                    try { await Context.Channel.SendErrorAsync(GetText("hangman_start_errored") + " " + ex.Message).ConfigureAwait(false); } catch { }
                    HangmanGames.TryRemove(Context.Channel.Id, out HangmanGame throwaway);
                    throwaway.Dispose();
                    return;
                }

                await Context.Channel.SendConfirmAsync(GetText("hangman_game_started"), hm.ScrambledWord + "\n" + hm.GetHangman());
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task HangmanStop()
            {
                if (HangmanGames.TryRemove(Context.Channel.Id, out HangmanGame throwaway))
                {
                    throwaway.Dispose();
                    await ReplyConfirmLocalized("hangman_stopped").ConfigureAwait(false);
                }
            }
        }
    }
}
