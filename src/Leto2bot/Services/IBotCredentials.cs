using Discord;
using System.Collections.Immutable;

namespace Leto2bot.Services
{
    public interface IBotCredentials
    {
        ulong ClientId { get; }

        string Token { get; }
        string GoogleApiKey { get; }
        ImmutableArray<ulong> OwnerIds { get; }
        string MashapeKey { get; }
        string PatreonAccessToken { get; }
        string CarbonKey { get; }

        DBConfig Db { get; }

        bool IsOwner(IUser u);
        int TotalShards { get; }
        string ShardRunCommand { get; }
        string ShardRunArguments { get; }
        string PatreonCampaignId { get; }
    }

    public class DBConfig
    {
        public DBConfig(string type, string connString)
        {
            this.Type = type;
            this.ConnectionString = connString;
        }
        public string Type { get; }
        public string ConnectionString { get; }
    }
}
