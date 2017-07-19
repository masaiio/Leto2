using System;

namespace Leto2bot.Services.Searches
{
    public class StreamNotFoundException : Exception
    {
        public StreamNotFoundException(string message) : base($"Stream '{message}' not found.")
        {
        }
    }
}
