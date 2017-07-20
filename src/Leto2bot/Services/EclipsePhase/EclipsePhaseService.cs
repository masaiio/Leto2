using Discord;
using Discord.WebSocket;
using Leto2bot.Extensions;
using Leto2bot.Services.Database.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Leto2bot.Services.EclipsePhase
{
    public class EclipsePhaseService
    {
        private readonly BotConfig _bc;


        private readonly DiscordSocketClient _client;
        private readonly LetoStrings _strings;
        private readonly IImagesService _images;
        private readonly Logger _log;

        private readonly CommandHandler _cmdHandler;

        public EclipsePhaseService(DiscordSocketClient client, BotConfig bc, IEnumerable<GuildConfig> gcs,
            LetoStrings strings, IImagesService images, CommandHandler cmdHandler)
        {
            _bc = bc;
            _client = client;
            _strings = strings;
            _images = images;
            _cmdHandler = cmdHandler;
            _log = LogManager.GetCurrentClassLogger();

        }

        public ConcurrentHashSet<ulong> GenerationChannels { get; }
        //channelid/message
        public ConcurrentDictionary<ulong, List<IUserMessage>> PlantedFlowers { get; } = new ConcurrentDictionary<ulong, List<IUserMessage>>();
        //channelId/last generation
        public ConcurrentDictionary<ulong, DateTime> LastGenerations { get; } = new ConcurrentDictionary<ulong, DateTime>();

        private ConcurrentDictionary<ulong, object> _locks { get; } = new ConcurrentDictionary<ulong, object>();

        public (string Name, ImmutableArray<byte> Data) GetRandomCurrencyImage()
        {
            var rng = new Leto2Random();
            return _images.Currency[rng.Next(0, _images.Currency.Length)];
        }

        private string GetText(ITextChannel ch, string key, params object[] rep)
            => _strings.GetText(key, ch.GuildId, "Games".ToLowerInvariant(), rep);

        private Task PotentialFlowerGeneration(SocketMessage imsg)
        {
            var msg = imsg as SocketUserMessage;
            if (msg == null || msg.Author.IsBot)
                return Task.CompletedTask;

            var channel = imsg.Channel as ITextChannel;
            if (channel == null)
                return Task.CompletedTask;

            if (!GenerationChannels.Contains(channel.Id))
                return Task.CompletedTask;

            var _ = Task.Run(async () =>
            {
                try
                {
                    var lastGeneration = LastGenerations.GetOrAdd(channel.Id, DateTime.MinValue);
                    var rng = new Leto2Random();

                    if (DateTime.UtcNow - TimeSpan.FromSeconds(_bc.CurrencyGenerationCooldown) < lastGeneration) //recently generated in this channel, don't generate again
                        return;

                    var num = rng.Next(1, 101) + _bc.CurrencyGenerationChance * 100;
                    if (num > 100 && LastGenerations.TryUpdate(channel.Id, DateTime.UtcNow, lastGeneration))
                    {
                        var dropAmount = _bc.CurrencyDropAmount;
                        var dropAmountMax = _bc.CurrencyDropAmountMax;

                        if (dropAmountMax != null && dropAmountMax > dropAmount)
                            dropAmount = new Leto2Random().Next(dropAmount, dropAmountMax.Value + 1);

                        if (dropAmount > 0)
                        {
                            var msgs = new IUserMessage[dropAmount];
                            var prefix = _cmdHandler.GetPrefix(channel.Guild.Id);
                            var toSend = dropAmount == 1
                                ? GetText(channel, "curgen_sn", _bc.CurrencySign)
                                    + " " + GetText(channel, "pick_sn", prefix)
                                : GetText(channel, "curgen_pl", dropAmount, _bc.CurrencySign)
                                    + " " + GetText(channel, "pick_pl", prefix);
                            var file = GetRandomCurrencyImage();
                            using (var fileStream = file.Data.ToStream())
                            {
                                var sent = await channel.SendFileAsync(
                                    fileStream,
                                    file.Name,
                                    toSend).ConfigureAwait(false);

                                msgs[0] = sent;
                            }

                            PlantedFlowers.AddOrUpdate(channel.Id, msgs.ToList(), (id, old) => { old.AddRange(msgs); return old; });
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.GetCurrentClassLogger().Warn(ex);
                }
            });
            return Task.CompletedTask;
        }
    }
}
