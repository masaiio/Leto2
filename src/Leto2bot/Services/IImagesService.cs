using System;
using System.Collections.Immutable;

namespace Leto2bot.Services
{
    public interface IImagesService
    {
        ImmutableArray<byte> Heads { get; }
        ImmutableArray<byte> Tails { get; }

        ImmutableArray<(string, ImmutableArray<byte>)> Currency { get; }
        ImmutableArray<ImmutableArray<byte>> Dice { get; }

        ImmutableArray<byte> SlotBackground { get; }
        ImmutableArray<ImmutableArray<byte>> SlotEmojis { get; }
        ImmutableArray<ImmutableArray<byte>> SlotNumbers { get; }

        void Reload();
    }
}
