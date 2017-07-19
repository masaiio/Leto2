using Leto2bot.Services.Database.Models;
using System.Collections.Generic;

namespace Leto2bot.Services.Database.Repositories
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        Currency GetOrCreate(ulong userId);
        long GetUserCurrency(ulong userId);
        bool TryUpdateState(ulong userId, long change);
        IEnumerable<Currency> GetTopRichest(int count, int skip);
    }
}
