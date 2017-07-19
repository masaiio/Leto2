using Leto2bot.Services.Database.Models;
using System.Collections;
using System.Collections.Generic;

namespace Leto2bot.Services.Database.Repositories
{
    public interface IReminderRepository : IRepository<Reminder>
    {
        IEnumerable<Reminder> GetIncludedReminders(List<long> guildIds);
    }
}
