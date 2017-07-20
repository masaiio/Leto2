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

        [Leto2Command, Usage, Description, Aliases] // EP Test command 
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

        [Leto2Command, Usage, Description, Aliases]  // EP Skill Test 
        public async Task EPTest(int num)
        {
            // check to make sure its a valid number 0-99
            if (num < 0 || num > 99)
            {
                await ReplyErrorLocalized("dice_invalid_number", 0, 99).ConfigureAwait(false);
                return;
            }

           //generate skill roll
            var rng = new Leto2Random();
            var gen = rng.Next(0, 100);
            int roll = gen; 
            
            //Compare roll to target number to check for sucess
            bool success = false; //true if successful
            bool benchmark = false; //true if +30 margin
            var type= "error"; // Type of sucesss or failure 
            var margin = 0; // for margin of sucess and margin of failure      
            
            if (num > roll) //sucess check
            {
                success = true;
                var margin = num - roll;
                if (margin > 30 ) {type = "Excellent Success";}
                else {type = "Success";}
            }
            else 
            {
                var margin = roll - num;
                if (margin > 30 ) {type = "Severe Failure";}
                else {type = "Failure";}
            }
            
            //check for critical (doubles)
            var critical = false; // true if critical roll 
            int[] digits = new int[2];
            for(int i = 1; i != 0; i--) // seperate tens and once place into seperate vars
            {
                digits[i] = (int)(num % 10);
                num = num / 10;
            }
            if (digets[0] == digets[1]) //check if the tens and ones place matches 
            {
                critical = true;
                type = "Critical " + type; 
            }      
            
            // send out result 
            if (success == true)
            {
            await Context.Channel.SendConfirmAsync(type + "!:", 
                "Rolled Value: " + roll.ToString() ", Test to Make: " + num.ToString(),
                "Margin of Sucess: +" + margin.ToString(),
                .ConfigureAwait(false);
            }
            else
            {
                 await Context.Channel.SendConfirmAsync(type + "!:", 
                "Rolled Value: " + roll.ToString() ", Test to Make: " + num.ToString(),
                "Margin of Failure: +" + margin.ToString(),
                .ConfigureAwait(false);
            }
        }
    }
}
