using Leto2bot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Leto2bot.Services.Database.Repositories.Impl
{
    public class CurrencyTransactionsRepository : Repository<CurrencyTransaction>, ICurrencyTransactionsRepository
    {
        public CurrencyTransactionsRepository(DbContext context) : base(context)
        {
        }
    }
}
