using Discord;
using Leto2bot.Services.Database.Models;
using System.Collections.Concurrent;

namespace Leto2bot.Services.Administration
{
    public enum ProtectionType
    {
        Raiding,
        Spamming,
    }

    public class AntiRaidStats
    {
        public AntiRaidSetting AntiRaidSettings { get; set; }
        public int UsersCount { get; set; }
        public ConcurrentHashSet<IGuildUser> RaidUsers { get; set; } = new ConcurrentHashSet<IGuildUser>();
    }

    public class AntiSpamStats
    {
        public AntiSpamSetting AntiSpamSettings { get; set; }
        public ConcurrentDictionary<ulong, UserSpamStats> UserStats { get; set; }
            = new ConcurrentDictionary<ulong, UserSpamStats>();
    }
}
