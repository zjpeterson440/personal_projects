namespace Warhammer.Rules.Core;

public enum RollKind
{
    ToHit,
    ToWound,
    ArmourSave,
    WardSave,
    Leadership,
    Psychology,
    Scatter,
    Artillery,
    Misc,
}

/// <summary>
/// One recorded roll. Carries enough context for the UI to present it the way
/// Baldur's Gate 3 does: resolved instantly, expandable if the player wants to
/// see why something happened.
/// </summary>
public sealed record DiceRoll(
    RollKind Kind,
    string Reason,
    IReadOnlyList<int> Values,
    int? Target = null,
    bool? Passed = null)
{
    public int Total => Values.Sum();

    public override string ToString()
    {
        string dice = $"[{string.Join(", ", Values)}]";
        string target = Target is null ? "" : $" vs {Target}+";
        string outcome = Passed is null ? "" : Passed.Value ? " — pass" : " — fail";
        return $"{Kind}: {Reason} {dice}{target}{outcome}";
    }
}

/// <summary>
/// The only source of randomness in the rules engine. Every roll is logged so
/// the presentation layer can replay it rather than recompute it.
/// </summary>
public sealed class Dice
{
    private readonly DeterministicRng _rng;
    private readonly List<DiceRoll> _log = new();

    public Dice(ulong seed) => _rng = new DeterministicRng(seed);

    public Dice(DeterministicRng rng) => _rng = rng;

    public IReadOnlyList<DiceRoll> Log => _log;

    public DeterministicRng Rng => _rng;

    public void ClearLog() => _log.Clear();

    /// <summary>Single D6, unmodified and untested.</summary>
    public int D6(RollKind kind, string reason)
    {
        int value = _rng.Die(6);
        _log.Add(new DiceRoll(kind, reason, new[] { value }));
        return value;
    }

    /// <summary>2D6, as used for Leadership and break tests.</summary>
    public int TwoD6(RollKind kind, string reason)
    {
        int a = _rng.Die(6);
        int b = _rng.Die(6);
        _log.Add(new DiceRoll(kind, reason, new[] { a, b }));
        return a + b;
    }

    /// <summary>
    /// Rolls a pool of D6 against a target, returning the number of successes.
    /// A natural 1 always fails and a natural 6 always succeeds, which is why
    /// the target is clamped rather than allowed to make a roll impossible or
    /// automatic.
    /// </summary>
    public int Pool(int count, int target, RollKind kind, string reason)
    {
        if (count <= 0)
        {
            return 0;
        }

        int clamped = Math.Clamp(target, 2, 6);
        var values = new int[count];
        int successes = 0;

        for (int i = 0; i < count; i++)
        {
            values[i] = _rng.Die(6);
            if (values[i] >= clamped)
            {
                successes++;
            }
        }

        _log.Add(new DiceRoll(kind, reason, values, clamped, successes > 0));
        return successes;
    }

    /// <summary>
    /// A 2D6 test against a characteristic — Leadership, and anything else that
    /// works the same way. Passes on equal or under.
    /// </summary>
    public bool TestAgainst(int characteristic, RollKind kind, string reason)
    {
        int a = _rng.Die(6);
        int b = _rng.Die(6);
        int total = a + b;
        bool passed = total <= characteristic;

        _log.Add(new DiceRoll(kind, reason, new[] { a, b }, characteristic, passed));
        return passed;
    }
}
