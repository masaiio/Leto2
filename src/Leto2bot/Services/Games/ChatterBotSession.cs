using Leto2bot.Extensions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Leto2bot.Services.Games
{
    public class ChatterBotSession
    {
        private static Leto2Random rng { get; } = new Leto2Random();
        public string ChatterbotId { get; }
        public string ChannelId { get; }
        private int _botId = 6;

        public ChatterBotSession(ulong channelId)
        {
            ChannelId = channelId.ToString().ToBase64();
            ChatterbotId = rng.Next(0, 1000000).ToString().ToBase64();
        }

        private string apiEndpoint => "http://api.program-o.com/v2/chatbot/" +
                                      $"?bot_id={_botId}&" +
                                      "say={0}&" +
                                      $"convo_id=leto2bot_{ChatterbotId}_{ChannelId}&" +
                                      "format=json";

        public async Task<string> Think(string message)
        {
            using (var http = new HttpClient())
            {
                var res = await http.GetStringAsync(string.Format(apiEndpoint, message)).ConfigureAwait(false);
                var cbr = JsonConvert.DeserializeObject<ChatterBotResponse>(res);
                return cbr.BotSay.Replace("<br/>", "\n");
            }
        }
    }
}
