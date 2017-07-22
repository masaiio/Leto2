using Discord.Commands;
using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Leto2bot.Services;
using System.Threading.Tasks;
using Leto2bot.Attributes;
using System;
using System.IO;
using System.Net;
using System.Linq;
using Leto2bot.Extensions;
using Leto2bot.Services.EclipsePhase;
using OAuth;


namespace Leto2bot.Modules.EclipsePhase
{
    public partial class EclipsePhase : Leto2TopLevelModule
    {
        private readonly EclipsePhaseService _eclipsephase;
        private readonly IBotCredentials _creds;
        private readonly IImagesService _images;

        public EclipsePhase(EclipsePhaseService eclipsePhase, IImagesService images)
        {
            _eclipsephase = eclipsePhase;
            _images = images;
        }

        public EclipsePhase(IBotCredentials creds)
        {
            _creds = creds;
        }

        //public static void FinishWebRequest(IAsyncResult result)
        //{
        //    HttpWebResponse response = (HttpWebResponse)(result.AsyncState as HttpWebRequest).EndGetResponse(result);
        //}

        //[Leto2Command, Usage, Description, Aliases]
        //public async Task ObsidianPortal([Remainder] string query = null)
        //{
        //    // check if keys are there 
        //    if (string.IsNullOrWhiteSpace(_creds.OAuthConsumerKey))
        //    {
        //        await ReplyErrorLocalized("mashape_api_missing").ConfigureAwait(false);
        //        return;
        //    }
        //    if (string.IsNullOrWhiteSpace(_creds.OAuthConsumerSecret))
        //    {
        //        await ReplyErrorLocalized("mashape_api_missing").ConfigureAwait(false);
        //        return;
        //    }
        //    //check if there is a argument passedd
        //    if (string.IsNullOrWhiteSpace(query))
        //        return;

        //    // Creating a new instance directly
        //    OAuthRequest client = new OAuthRequest
        //    {
        //        Method = "GET",
        //        Type = OAuthRequestType.RequestToken,
        //        SignatureMethod = OAuthSignatureMethod.HmacSha1,
        //        ConsumerKey = "",
        //        ConsumerSecret = "",
        //        RequestUrl = "https://www.obsidianportal.com/oauth/request_token",
        //        Version = "1.0a",
        //        Realm = "ObsidianPortal.com"
        //    };


        //    // Using HTTP header authorization
        //    string auth = client.GetAuthorizationQuery();
        //    var url = client.RequestUrl + "?" + auth;

        //    var request = (HttpWebRequest)WebRequest.Create(url);
        //   request.BeginGetResponse(new AsyncCallback(FinishWebRequest), request);

        //    Console.Write(auth);
        //    Console.ReadLine(); //idk what tihs is 

        //    await Context.Channel.TriggerTypingAsync().ConfigureAwait(false);

        //    // the authentication check?
            
        //    using (StreamReader SR = new StreamReader(response.GetResponseStream()))
        //    {
        //        Console.Write(SR.ReadToEnd());
        //        var res = await client.GetStringAsync($"http://api.obsidianportal.com/v1/users/me.json").ConfigureAwait(false);
        //        try
        //        {
        //            var items = JObject.Parse(res);
        //            var item = items["list"][0];
        //            var link = item["profile_url"].ToString();
        //            var embed = new EmbedBuilder().WithOkColor()
        //                             .WithUrl(link);
        //            await Context.Channel.EmbedAsync(embed).ConfigureAwait(false);
        //        }
        //        catch
        //        {
        //            await ReplyErrorLocalized("ud_error").ConfigureAwait(false);
        //        }
        //    }
        //}

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
            int test = num;
            var type = "error"; // Type of sucesss or failure 
            var margin = 0; // for margin of sucess and margin of failure      
            
            if (test > roll) //sucess check
            {
                success = true;
                margin = test - roll;
                if (margin >= 30 ) {type = "Excellent Success";}
                else {type = "Success";}
            }
            else 
            {
                margin = roll - test;
                if (margin >= 30 ) {type = "Severe Failure";}
                else {type = "Failure";}
            }
            
            //check for critical (doubles)
            int[] digits = new int[2];
            digits[1] = roll / 10;
            digits[0] = roll - (digits[1] * 10);
            if (digits[0] == digits[1]) //check if the tens and ones place matches 
            {
                type = "Critical " + type; 
            }

            // send out result 
            if (success == true)
            {
                await Context.Channel.SendConfirmAsync(type + "!:",
                    "Rolled Value: " + roll.ToString() + ", Test to Make: " + test.ToString() +
                    ", Margin of Sucess: +" + margin.ToString())
                    .ConfigureAwait(false);
            }
            else
            {
                await Context.Channel.SendConfirmAsync(type + "!:",
               "Rolled Value: " + roll.ToString() + ", Test to Make: " + test.ToString() +
               ", Margin of Failure: +" + margin.ToString())
                .ConfigureAwait(false);
            }
        }
    }
}
