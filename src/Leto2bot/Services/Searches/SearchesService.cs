using Discord;
using Discord.WebSocket;
using Leto2bot.DataStructures;
using Leto2bot.Extensions;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Leto2bot.Services.Searches
{
    public class SearchesService
    {
        private readonly DiscordSocketClient _client;
        private readonly IGoogleApiService _google;
        private readonly DbService _db;
        private readonly Logger _log;

        public ConcurrentDictionary<ulong, bool> TranslatedChannels { get; } = new ConcurrentDictionary<ulong, bool>();
        public ConcurrentDictionary<UserChannelPair, string> UserLanguages { get; } = new ConcurrentDictionary<UserChannelPair, string>();
        

        private readonly ConcurrentDictionary<ulong?, SearchImageCacher> _imageCacher = new ConcurrentDictionary<ulong?, SearchImageCacher>();

        public SearchesService(DiscordSocketClient client, IGoogleApiService google, DbService db)
        {
            _client = client;
            _google = google;
            _db = db;
            _log = LogManager.GetCurrentClassLogger();

            //translate commands
            _client.MessageReceived += (msg) =>
            {
                var _ = Task.Run(async () =>
                {
                    try
                    {
                        var umsg = msg as SocketUserMessage;
                        if (umsg == null)
                            return;

                        if (!TranslatedChannels.TryGetValue(umsg.Channel.Id, out var autoDelete))
                            return;

                        var key = new UserChannelPair()
                        {
                            UserId = umsg.Author.Id,
                            ChannelId = umsg.Channel.Id,
                        };

                        if (!UserLanguages.TryGetValue(key, out string langs))
                            return;

                        var text = await Translate(langs, umsg.Resolve(TagHandling.Ignore))
                                            .ConfigureAwait(false);
                        if (autoDelete)
                            try { await umsg.DeleteAsync().ConfigureAwait(false); } catch { }
                        await umsg.Channel.SendConfirmAsync($"{umsg.Author.Mention} `:` " + text.Replace("<@ ", "<@").Replace("<@! ", "<@!")).ConfigureAwait(false);
                    }
                    catch { }
                });
                return Task.CompletedTask;
            };


            //joke commands
        }

        public async Task<string> Translate(string langs, string text = null)
        {
            var langarr = langs.ToLowerInvariant().Split('>');
            if (langarr.Length != 2)
                throw new ArgumentException();
            var from = langarr[0];
            var to = langarr[1];
            text = text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException();
            return (await _google.Translate(text, from, to).ConfigureAwait(false)).SanitizeMentions();
        }

        public Task<ImageCacherObject> DapiSearch(string tag, DapiSearchType type, ulong? guild, bool isExplicit = false)
        {
            var cacher = _imageCacher.GetOrAdd(guild, (key) => new SearchImageCacher());
            
            return cacher.GetImage(tag, isExplicit, type);
        }
    }
    
    public struct UserChannelPair
    {
        public ulong UserId { get; set; }
        public ulong ChannelId { get; set; }
    }

    public class StreamStatus
    {
        public bool IsLive { get; set; }
        public string ApiLink { get; set; }
        public string Views { get; set; }
    }
}
