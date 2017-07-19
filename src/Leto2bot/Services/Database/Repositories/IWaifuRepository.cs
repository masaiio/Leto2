using Leto2bot.Services.Database.Models;
using System.Collections.Generic;

namespace Leto2bot.Services.Database.Repositories
{
    public interface IWaifuRepository : IRepository<WaifuInfo>
    {
        IList<WaifuInfo> GetTop(int count, int skip = 0);
        WaifuInfo ByWaifuUserId(ulong userId);
        IList<WaifuInfo> ByClaimerUserId(ulong userId);
    }
}
