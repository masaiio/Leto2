using Leto2bot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leto2bot.Services.Database.Repositories.Impl
{
    public class ReminderRepository : Repository<Reminder>, IReminderRepository
    {
        public ReminderRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<Reminder> GetIncludedReminders(List<long> guildIds)
        {
            return _set.Where(x => guildIds.Contains((long)x.ServerId)).ToList();
        }
    }
}
