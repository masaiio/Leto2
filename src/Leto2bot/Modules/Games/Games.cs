using Discord.Commands;
using Discord;
using Leto2bot.Services;
using System.Threading.Tasks;
using Leto2bot.Attributes;
using System;
using Leto2bot.Extensions;
using Leto2bot.Services.Games;

namespace Leto2bot.Modules.Games
{
    public partial class Games : Leto2TopLevelModule
    {
        private readonly GamesService _games;
        private readonly IImagesService _images;

        public Games(GamesService games, IImagesService images)
        {
            _games = games;
            _images = images;
        }

        [Leto2Command, Usage, Description, Aliases]
        public async Task Choose([Remainder] string list = null)
        {
            if (string.IsNullOrWhiteSpace(list))
                return;
            var listArr = list.Split(';');
            if (listArr.Length < 2)
                return;
            var rng = new Leto2Random();
            await Context.Channel.SendConfirmAsync("🤔", listArr[rng.Next(0, listArr.Length)]).ConfigureAwait(false);
        }

        [Leto2Command, Usage, Description, Aliases]
        public async Task _8Ball([Remainder] string question = null)
        {
            if (string.IsNullOrWhiteSpace(question))
                return;

            await Context.Channel.EmbedAsync(new EmbedBuilder().WithColor(Leto2bot.OkColor)
                               .AddField(efb => efb.WithName("❓ " + GetText("question") ).WithValue(question).WithIsInline(false))
                               .AddField(efb => efb.WithName("🎱 " + GetText("8ball")).WithValue(_games.EightBallResponses[new Leto2Random().Next(0, _games.EightBallResponses.Length)]).WithIsInline(false)));
        }

        [Leto2Command, Usage, Description, Aliases]
        public async Task Rps(string input)
        {
            Func<int,string> getRpsPick = (p) =>
            {
                switch (p)
                {
                    case 0:
                        return "🚀";
                    case 1:
                        return "📎";
                    default:
                        return "✂️";
                }
            };

            int pick;
            switch (input)
            {
                case "r":
                case "rock":
                case "rocket":
                    pick = 0;
                    break;
                case "p":
                case "paper":
                case "paperclip":
                    pick = 1;
                    break;
                case "scissors":
                case "s":
                    pick = 2;
                    break;
                default:
                    return;
            }
            var leto2Pick = new Leto2Random().Next(0, 3);
            string msg;
            if (pick == leto2Pick)
                msg = GetText("rps_draw", getRpsPick(pick));
            else if ((pick == 0 && leto2Pick == 1) ||
                     (pick == 1 && leto2Pick == 2) ||
                     (pick == 2 && leto2Pick == 0))
                msg = GetText("rps_win", Context.Client.CurrentUser.Mention,
                    getRpsPick(leto2Pick), getRpsPick(pick));
            else
                msg = GetText("rps_win", Context.User.Mention, getRpsPick(pick),
                    getRpsPick(leto2Pick));

            await Context.Channel.SendConfirmAsync(msg).ConfigureAwait(false);
        }

        private double NextDouble(double x, double y)
        {
            var rng = new Random();
            return rng.NextDouble() * (y - x) + x;
        }

    }
}
