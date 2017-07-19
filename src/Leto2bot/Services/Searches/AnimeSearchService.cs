using Newtonsoft.Json;
using NLog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Leto2bot.Services.Searches
{
    public class AnimeSearchService
    {
        private readonly Logger _log;

        public AnimeSearchService()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public async Task<AnimeResult> GetAnimeData(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));
            try
            {

                var link = "https://aniapi.leto2bot.me/anime/" + Uri.EscapeDataString(query.Replace("/", " "));
                using (var http = new HttpClient())
                {
                    var res = await http.GetStringAsync(link).ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<AnimeResult>(res);
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<MangaResult> GetMangaData(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));
            try
            {

                var link = "https://aniapi.leto2bot.me/manga/" + Uri.EscapeDataString(query.Replace("/", " "));
                using (var http = new HttpClient())
                {
                    var res = await http.GetStringAsync(link).ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<MangaResult>(res);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
