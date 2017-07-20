using Discord.Commands;
using Discord;
using Leto2bot.Services;
using System.Threading.Tasks;
using Leto2bot.Attributes;
using System;
using Leto2bot.Extensions;
using Leto2bot.Services.EclipsePhase;

namespace Leto2bot.Modules.EclipsePhase
{
    public partial class EclipsePhase : Leto2TopLevelModule
    {
        private readonly EclipsePhaseService _eclipsephase;
        private readonly IImagesService _images;

        public EclipsePhase(EclipsePhaseService eclipsePhase, IImagesService images)
        {
            _eclipsephase = eclipsePhase;
            _images = images;
        }

        [Leto2Command, Usage, Description, Aliases]
        public async Task EPChoose([Remainder] string list = null)
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
        public async Task EPTest(int num)
        {
            if (num < 0 || num > 99)
            {
                await ReplyErrorLocalized("dice_invalid_number", 0, 99).ConfigureAwait(false);
                return;
            }

           //generate skill roll
            var rng = new Leto2Random();
            var gen = rng.Next(0, 100);

            await Context.Channel.SendConfirmAsync(gen.ToString(), num.ToString()).ConfigureAwait(false);
        }

    }
}
