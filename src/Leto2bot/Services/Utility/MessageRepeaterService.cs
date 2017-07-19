using Discord.WebSocket;
using Leto2bot.Services.Database.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leto2bot.Services.Utility
{
    //todo 50 rewrite
    public class MessageRepeaterService
    {
        //messagerepeater
        //guildid/RepeatRunners
        public ConcurrentDictionary<ulong, ConcurrentQueue<RepeatRunner>> Repeaters { get; set; }
        public bool RepeaterReady { get; private set; }

        public MessageRepeaterService(Leto2bot bot, DiscordSocketClient client, IEnumerable<GuildConfig> gcs)
        {
            var _ = Task.Run(async () =>
            {
                while (!bot.Ready)
                    await Task.Delay(1000);

                Repeaters = new ConcurrentDictionary<ulong, ConcurrentQueue<RepeatRunner>>(gcs
                    .ToDictionary(gc => gc.GuildId,
                        gc => new ConcurrentQueue<RepeatRunner>(gc.GuildRepeaters
                            .Select(gr => new RepeatRunner(client, gr))
                            .Where(x => x.Guild != null))));
                RepeaterReady = true;
            });
        }
    }
}
