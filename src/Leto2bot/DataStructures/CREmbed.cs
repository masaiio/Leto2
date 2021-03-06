﻿using Discord;
using Leto2bot.Extensions;
using Newtonsoft.Json;
using NLog;
using System;

namespace Leto2bot.DataStructures
{
    public class CREmbed
    {
        private static readonly Logger _log;
        public string PlainText { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public CREmbedFooter Footer { get; set; }
        public string Thumbnail { get; set; }
        public string Image { get; set; }
        public CREmbedField[] Fields { get; set; }
        public uint Color { get; set; } = 7458112;

        static CREmbed()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Title) ||
            !string.IsNullOrWhiteSpace(Description) ||
            !string.IsNullOrWhiteSpace(Thumbnail) ||
            !string.IsNullOrWhiteSpace(Image) ||
            (Footer != null && (!string.IsNullOrWhiteSpace(Footer.Text) || !string.IsNullOrWhiteSpace(Footer.IconUrl))) ||
            (Fields != null && Fields.Length > 0);

        public EmbedBuilder ToEmbed()
        {
            var embed = new EmbedBuilder();

            if (!string.IsNullOrWhiteSpace(Title))
                embed.WithTitle(Title);
            if (!string.IsNullOrWhiteSpace(Description))
                embed.WithDescription(Description);
            embed.WithColor(new Discord.Color(Color));
            if (Footer != null)
                embed.WithFooter(efb =>
                {
                    efb.WithText(Footer.Text);
                    if (Uri.IsWellFormedUriString(Footer.IconUrl, UriKind.Absolute))
                        efb.WithIconUrl(Footer.IconUrl);
                });

            if (Thumbnail != null && Uri.IsWellFormedUriString(Thumbnail, UriKind.Absolute))
                embed.WithThumbnailUrl(Thumbnail);
            if(Image != null && Uri.IsWellFormedUriString(Image, UriKind.Absolute))
                embed.WithImageUrl(Image);

            if (Fields != null)
                foreach (var f in Fields)
                {
                    if(!string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Value))
                        embed.AddField(efb => efb.WithName(f.Name).WithValue(f.Value).WithIsInline(f.Inline));
                }

            return embed;
        }

        public static bool TryParse(string input, out CREmbed embed)
        {
            embed = null;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                var crembed = JsonConvert.DeserializeObject<CREmbed>(input);
                
                if(crembed.Fields != null && crembed.Fields.Length > 0)
                    foreach (var f in crembed.Fields)
                    {
                        f.Name = f.Name.TrimTo(256);
                        f.Value = f.Value.TrimTo(1024);
                    }
                if (!crembed.IsValid)
                    return false;

                embed = crembed;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class CREmbedField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Inline { get; set; }
    }

    public class CREmbedFooter {
        public string Text { get; set; }
        public string IconUrl { get; set; }
    }
}
