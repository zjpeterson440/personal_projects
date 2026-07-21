namespace Warhammer.Rules.Core;

/// <summary>
/// A Warhammer characteristic profile. Movement is in inches; everything else
/// is a raw characteristic value.
/// </summary>
public readonly record struct Profile(
    int M,
    int WS,
    int BS,
    int S,
    int T,
    int W,
    int I,
    int A,
    int Ld)
{
    public override string ToString() =>
        $"M{M} WS{WS} BS{BS} S{S} T{T} W{W} I{I} A{A} Ld{Ld}";
}

public enum UnitCategory
{
    Infantry,
    Cavalry,
    WarMachine,
    Character,
    Monster,
    Chariot,
}

/// <summary>
/// Placeholder profiles so the rules core can be exercised before the army
/// books are encoded. These are shaped correctly but the values are NOT
/// authoritative — see docs/rules-to-verify.md.
/// </summary>
public static class PlaceholderProfiles
{
    public static readonly Profile DwarfWarrior =
        new(M: 3, WS: 4, BS: 3, S: 3, T: 4, W: 1, I: 2, A: 1, Ld: 9);

    public static readonly Profile DwarfThane =
        new(M: 3, WS: 6, BS: 4, S: 4, T: 5, W: 2, I: 3, A: 3, Ld: 9);

    public static readonly Profile HighElfSpearman =
        new(M: 5, WS: 4, BS: 4, S: 3, T: 3, W: 1, I: 5, A: 1, Ld: 8);

    public static readonly Profile HighElfNoble =
        new(M: 5, WS: 6, BS: 6, S: 4, T: 3, W: 2, I: 7, A: 3, Ld: 9);
}
