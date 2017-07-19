﻿using Discord;
using Discord.Commands;
using Leto2bot.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Leto2bot.Services;
using System.Net.Http;
using Leto2bot.Extensions;
using System.Xml;
using System.Threading;
using System.Collections.Concurrent;
using Leto2bot.Services.Searches;
using Leto2bot.DataStructures;

namespace Leto2bot.Modules.NSFW
{
    public class NSFW : Leto2TopLevelModule
    {
        private static readonly ConcurrentDictionary<ulong, Timer> _autoHentaiTimers = new ConcurrentDictionary<ulong, Timer>();
        private static readonly ConcurrentHashSet<ulong> _hentaiBombBlacklist = new ConcurrentHashSet<ulong>();
        private readonly SearchesService _service;

        public NSFW(SearchesService service)
        {
            _service = service;
        }

        private async Task InternalHentai(IMessageChannel channel, string tag, bool noError)
        {
            var rng = new Leto2Random();
            var arr = Enum.GetValues(typeof(DapiSearchType));
            var type = (DapiSearchType)arr.GetValue(new Leto2Random().Next(2, arr.Length));
            var img = await _service.DapiSearch(tag, type, Context.Guild?.Id, true).ConfigureAwait(false);

            if (img == null)
            {
                if (!noError)
                    await ReplyErrorLocalized("not_found").ConfigureAwait(false);
                return;
            }

            await channel.EmbedAsync(new EmbedBuilder().WithOkColor()
                .WithImageUrl(img.FileUrl)
                .WithDescription($"[{GetText("tag")}: {tag}]({img})"))
                .ConfigureAwait(false);
        }

        [Leto2Command, Usage, Description, Aliases]
        public Task Hentai([Remainder] string tag = null) =>
            InternalHentai(Context.Channel, tag, false);
#if !GLOBAL_NADEKO
        [Leto2Command, Usage, Description, Aliases]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task AutoHentai(int interval = 0, string tags = null)
        {
            Timer t;

            if (interval == 0)
            {
                if (!_autoHentaiTimers.TryRemove(Context.Channel.Id, out t)) return;

                t.Change(Timeout.Infinite, Timeout.Infinite); //proper way to disable the timer
                await ReplyConfirmLocalized("autohentai_stopped").ConfigureAwait(false);
                return;
            }

            if (interval < 20)
                return;

            var tagsArr = tags?.Split('|');

            t = new Timer(async (state) =>
            {
                try
                {
                    if (tagsArr == null || tagsArr.Length == 0)
                        await InternalHentai(Context.Channel, null, true).ConfigureAwait(false);
                    else
                        await InternalHentai(Context.Channel, tagsArr[new Leto2Random().Next(0, tagsArr.Length)], true).ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }
            }, null, interval * 1000, interval * 1000);

            _autoHentaiTimers.AddOrUpdate(Context.Channel.Id, t, (key, old) =>
            {
                old.Change(Timeout.Infinite, Timeout.Infinite);
                return t;
            });

            await ReplyConfirmLocalized("autohentai_started",
                interval,
                string.Join(", ", tagsArr)).ConfigureAwait(false);
        }
#endif

        [Leto2Command, Usage, Description, Aliases]
        public async Task HentaiBomb([Remainder] string tag = null)
        {
            if (!_hentaiBombBlacklist.Add(Context.Guild?.Id ?? Context.User.Id))
                return;
            try
            {
                var images = await Task.WhenAll(_service.DapiSearch(tag, DapiSearchType.Gelbooru, Context.Guild?.Id, true),
                                                _service.DapiSearch(tag, DapiSearchType.Danbooru, Context.Guild?.Id, true),
                                                _service.DapiSearch(tag, DapiSearchType.Konachan, Context.Guild?.Id, true),
                                                _service.DapiSearch(tag, DapiSearchType.Yandere, Context.Guild?.Id, true)).ConfigureAwait(false);

                var linksEnum = images?.Where(l => l != null).ToArray();
                if (images == null || !linksEnum.Any())
                {
                    await ReplyErrorLocalized("not_found").ConfigureAwait(false);
                    return;
                }

                await Context.Channel.SendMessageAsync(string.Join("\n\n", linksEnum.Select(x => x.FileUrl))).ConfigureAwait(false);
            }
            finally
            {
                _hentaiBombBlacklist.TryRemove(Context.Guild?.Id ?? Context.User.Id);
            }
        }

        [Leto2Command, Usage, Description, Aliases]
        public Task Yandere([Remainder] string tag = null)
            => InternalDapiCommand(tag, DapiSearchType.Yandere, false);

        [Leto2Command, Usage, Description, Aliases]
        public Task Konachan([Remainder] string tag = null)
            => InternalDapiCommand(tag, DapiSearchType.Konachan, false);

        [Leto2Command, Usage, Description, Aliases]
        public Task E621([Remainder] string tag = null)
            => InternalDapiCommand(tag, DapiSearchType.E621, false);

        [Leto2Command, Usage, Description, Aliases]
        public Task Rule34([Remainder] string tag = null)
            => InternalDapiCommand(tag, DapiSearchType.Rule34, false);

        [Leto2Command, Usage, Description, Aliases]
        public Task Danbooru([Remainder] string tag = null)
            => InternalDapiCommand(tag, DapiSearchType.Danbooru, false);

        [Leto2Command, Usage, Description, Aliases]
        public Task Gelbooru([Remainder] string tag = null)
            => InternalDapiCommand(tag, DapiSearchType.Gelbooru, false);

        [Leto2Command, Usage, Description, Aliases]
        public async Task Boobs()
        {
            try
            {
                JToken obj;
                using (var http = new HttpClient())
                {
                    obj = JArray.Parse(await http.GetStringAsync($"http://api.oboobs.ru/boobs/{new Leto2Random().Next(0, 10330)}").ConfigureAwait(false))[0];
                }
                await Context.Channel.SendMessageAsync($"http://media.oboobs.ru/{obj["preview"]}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await Context.Channel.SendErrorAsync(ex.Message).ConfigureAwait(false);
            }
        }

        [Leto2Command, Usage, Description, Aliases]
        public async Task Butts()
        {
            try
            {
                JToken obj;
                using (var http = new HttpClient())
                {
                    obj = JArray.Parse(await http.GetStringAsync($"http://api.obutts.ru/butts/{new Leto2Random().Next(0, 4335)}").ConfigureAwait(false))[0];
                }
                await Context.Channel.SendMessageAsync($"http://media.obutts.ru/{obj["preview"]}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await Context.Channel.SendErrorAsync(ex.Message).ConfigureAwait(false);
            }
        }

        public async Task InternalDapiCommand(string tag, DapiSearchType type, bool forceExplicit)
        {
            var imgObj = await _service.DapiSearch(tag, type, Context.Guild?.Id, forceExplicit).ConfigureAwait(false);

            if (imgObj == null)
                await ReplyErrorLocalized("not_found").ConfigureAwait(false);
            else
                await Context.Channel.EmbedAsync(new EmbedBuilder().WithOkColor()
                    .WithDescription($"{Context.User} [{tag ?? "url"}]({imgObj}) ")
                    .WithImageUrl(imgObj.FileUrl)
                    .WithFooter(efb => efb.WithText(type.ToString()))).ConfigureAwait(false);
        }
    }
}