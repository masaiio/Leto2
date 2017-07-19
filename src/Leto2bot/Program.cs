﻿namespace Leto2bot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 3 && int.TryParse(args[0], out int shardId) && int.TryParse(args[1], out int parentProcessId))
            {
                int? port = null;
                if (int.TryParse(args[2], out var outPort))
                    port = outPort;
                new Leto2bot(shardId, parentProcessId, outPort).RunAndBlockAsync(args).GetAwaiter().GetResult();
            }
            else
                new Leto2bot(0, 0).RunAndBlockAsync(args).GetAwaiter().GetResult();
        }
    }
}
