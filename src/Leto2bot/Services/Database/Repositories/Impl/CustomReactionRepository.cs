using Leto2bot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Leto2bot.Services.Database.Repositories.Impl
{
    public class CustomReactionsRepository : Repository<CustomReaction>, ICustomReactionRepository
    {
        public CustomReactionsRepository(DbContext context) : base(context)
        {
        }
    }
}
