namespace Warhammer.Rules.Core;

/// <summary>
/// xoshiro256** seeded via SplitMix64.
/// </summary>
/// <remarks>
/// <see cref="System.Random"/> is deliberately avoided: its algorithm is not
/// guaranteed stable across .NET versions, which would silently break replays
/// and desync lockstep multiplayer. This implementation is fixed forever, so a
/// seed plus a command list always reproduces the same game.
/// </remarks>
public sealed class DeterministicRng
{
    private ulong _s0, _s1, _s2, _s3;

    public DeterministicRng(ulong seed)
    {
        // Expand the seed across the full 256-bit state. SplitMix64 is the
        // author-recommended seeder for xoshiro and avoids the poor early
        // output that comes from a state that is mostly zeroes.
        _s0 = SplitMix64(ref seed);
        _s1 = SplitMix64(ref seed);
        _s2 = SplitMix64(ref seed);
        _s3 = SplitMix64(ref seed);

        // An all-zero state is a fixed point that only ever emits zero.
        if ((_s0 | _s1 | _s2 | _s3) == 0)
        {
            _s0 = 0x9E3779B97F4A7C15UL;
        }
    }

    private DeterministicRng(ulong s0, ulong s1, ulong s2, ulong s3)
    {
        _s0 = s0;
        _s1 = s1;
        _s2 = s2;
        _s3 = s3;
    }

    private static ulong SplitMix64(ref ulong x)
    {
        x += 0x9E3779B97F4A7C15UL;
        ulong z = x;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }

    private static ulong Rotl(ulong x, int k) => (x << k) | (x >> (64 - k));

    public ulong NextUInt64()
    {
        ulong result = Rotl(_s1 * 5, 7) * 9;

        ulong t = _s1 << 17;
        _s2 ^= _s0;
        _s3 ^= _s1;
        _s1 ^= _s2;
        _s0 ^= _s3;
        _s2 ^= t;
        _s3 = Rotl(_s3, 45);

        return result;
    }

    /// <summary>
    /// Uniform integer in [0, bound), using Lemire's multiply-shift with
    /// rejection. Plain modulo would bias low results; for d6 the bias is tiny
    /// but it is free to be exact, and exactness matters when the whole point
    /// is that players trust the dice.
    /// </summary>
    public uint NextBelow(uint bound)
    {
        if (bound == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bound), "Bound must be positive.");
        }

        uint random = (uint)(NextUInt64() >> 32);
        ulong product = (ulong)random * bound;
        uint low = (uint)product;

        if (low < bound)
        {
            uint threshold = unchecked(0u - bound) % bound;
            while (low < threshold)
            {
                random = (uint)(NextUInt64() >> 32);
                product = (ulong)random * bound;
                low = (uint)product;
            }
        }

        return (uint)(product >> 32);
    }

    /// <summary>Rolls a single die with the given number of sides.</summary>
    public int Die(int sides) => (int)NextBelow((uint)sides) + 1;

    /// <summary>
    /// Snapshots the generator state so a game can be saved mid-turn and
    /// resumed bit-identically.
    /// </summary>
    public RngState Capture() => new(_s0, _s1, _s2, _s3);

    public static DeterministicRng Restore(RngState state) =>
        new(state.S0, state.S1, state.S2, state.S3);
}

public readonly record struct RngState(ulong S0, ulong S1, ulong S2, ulong S3);
