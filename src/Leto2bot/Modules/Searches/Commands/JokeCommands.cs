using AngleSharp;
using Discord.Commands;
using Leto2bot.Attributes;
using Leto2bot.Extensions;
using Leto2bot.Services;
using Leto2bot.Services.Searches;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Leto2bot.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class JokeCommands : Leto2Submodule
        {
            private readonly SearchesService _searches;

            public JokeCommands(SearchesService searches)
            {
                _searches = searches;
            }

            [Leto2Command, Usage, Description, Aliases]
            public async Task Randjoke()
            {
                using (var http = new HttpClient())
                {
                    http.AddFakeHeaders();

                    var config = Configuration.Default.WithDefaultLoader();
                    var document = await BrowsingContext.New(config).OpenAsync("http://www.goodbadjokes.com/random");

                    var html = document.QuerySelector(".post > .joke-content");

                    var part1 = html.QuerySelector("dt").TextContent;
                    var part2 = html.QuerySelector("dd").TextContent;

                    await Context.Channel.SendConfirmAsync("", part1 + "\n\n" + part2, footer: document.BaseUri).ConfigureAwait(false);
                }
            }
        }
    }
}
