using Discord.Commands;
using System;

namespace Leto2bot.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    sealed class Leto2ModuleAttribute : GroupAttribute
    {
        public Leto2ModuleAttribute(string moduleName) : base(moduleName)
        {
        }
    }
}

