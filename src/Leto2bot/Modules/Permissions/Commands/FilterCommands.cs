﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Leto2bot.Attributes;
using Leto2bot.Extensions;
using Leto2bot.Services;
using Leto2bot.Services.Permissions;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Permissions
{
    public partial class Permissions
    {
        [Group]
        public class FilterCommands : Leto2Submodule
        {
            private readonly DbService _db;
            private readonly FilterService _service;

            public FilterCommands(FilterService service, DbService db)
            {
                _service = service;
                _db = db;
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task SrvrFilterInv()
            {
                var channel = (ITextChannel)Context.Channel;

                bool enabled;
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.GuildConfigs.For(channel.Guild.Id, set => set);
                    enabled = config.FilterInvites = !config.FilterInvites;
                    await uow.CompleteAsync().ConfigureAwait(false);
                }
                
                if (enabled)
                {
                    _service.InviteFilteringServers.Add(channel.Guild.Id);
                    await ReplyConfirmLocalized("invite_filter_server_on").ConfigureAwait(false);
                }
                else
                {
                    _service.InviteFilteringServers.TryRemove(channel.Guild.Id);
                    await ReplyConfirmLocalized("invite_filter_server_off").ConfigureAwait(false);
                }
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task ChnlFilterInv()
            {
                var channel = (ITextChannel)Context.Channel;

                int removed;
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.GuildConfigs.For(channel.Guild.Id, set => set.Include(gc => gc.FilterInvitesChannelIds));
                    removed = config.FilterInvitesChannelIds.RemoveWhere(fc => fc.ChannelId == channel.Id);
                    if (removed == 0)
                    {
                        config.FilterInvitesChannelIds.Add(new Services.Database.Models.FilterChannelId()
                        {
                            ChannelId = channel.Id
                        });
                    }
                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                if (removed == 0)
                {
                    _service.InviteFilteringChannels.Add(channel.Id);
                    await ReplyConfirmLocalized("invite_filter_channel_on").ConfigureAwait(false);
                }
                else
                {
                    await ReplyConfirmLocalized("invite_filter_channel_off").ConfigureAwait(false);
                }
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task SrvrFilterWords()
            {
                var channel = (ITextChannel)Context.Channel;

                bool enabled;
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.GuildConfigs.For(channel.Guild.Id, set => set);
                    enabled = config.FilterWords = !config.FilterWords;
                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                if (enabled)
                {
                    _service.WordFilteringServers.Add(channel.Guild.Id);
                    await ReplyConfirmLocalized("word_filter_server_on").ConfigureAwait(false);
                }
                else
                {
                    _service.WordFilteringServers.TryRemove(channel.Guild.Id);
                    await ReplyConfirmLocalized("word_filter_server_off").ConfigureAwait(false);
                }
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task ChnlFilterWords()
            {
                var channel = (ITextChannel)Context.Channel;

                int removed;
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.GuildConfigs.For(channel.Guild.Id, set => set.Include(gc => gc.FilterWordsChannelIds));
                    removed = config.FilterWordsChannelIds.RemoveWhere(fc => fc.ChannelId == channel.Id);
                    if (removed == 0)
                    {
                        config.FilterWordsChannelIds.Add(new Services.Database.Models.FilterChannelId()
                        {
                            ChannelId = channel.Id
                        });
                    }
                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                if (removed == 0)
                {
                    _service.WordFilteringChannels.Add(channel.Id);
                    await ReplyConfirmLocalized("word_filter_channel_on").ConfigureAwait(false);
                }
                else
                {
                    _service.WordFilteringChannels.TryRemove(channel.Id);
                    await ReplyConfirmLocalized("word_filter_channel_off").ConfigureAwait(false);
                }
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task FilterWord([Remainder] string word)
            {
                var channel = (ITextChannel)Context.Channel;

                word = word?.Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(word))
                    return;

                int removed;
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.GuildConfigs.For(channel.Guild.Id, set => set.Include(gc => gc.FilteredWords));

                    removed = config.FilteredWords.RemoveWhere(fw => fw.Word.Trim().ToLowerInvariant() == word);

                    if (removed == 0)
                        config.FilteredWords.Add(new Services.Database.Models.FilteredWord() { Word = word });

                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                var filteredWords = _service.ServerFilteredWords.GetOrAdd(channel.Guild.Id, new ConcurrentHashSet<string>());

                if (removed == 0)
                {
                    filteredWords.Add(word);
                    await ReplyConfirmLocalized("filter_word_add", Format.Code(word)).ConfigureAwait(false);
                }
                else
                {
                    filteredWords.TryRemove(word);
                    await ReplyConfirmLocalized("filter_word_remove", Format.Code(word)).ConfigureAwait(false);
                }
            }

            [Leto2Command, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task LstFilterWords(int page = 1)
            {
                page--;
                if (page < 0)
                    return;

                var channel = (ITextChannel)Context.Channel;

                _service.ServerFilteredWords.TryGetValue(channel.Guild.Id, out var fwHash);

                var fws = fwHash.ToArray();

                await channel.SendPaginatedConfirmAsync((DiscordSocketClient)Context.Client,
                    page,
                    (curPage) =>
                        new EmbedBuilder()
                            .WithTitle(GetText("filter_word_list"))
                            .WithDescription(string.Join("\n", fws.Skip(curPage * 10).Take(10)))
                , fws.Length / 10).ConfigureAwait(false);
            }
        }
    }
}
