using Warhammer.Rules.Core;
using Xunit;

namespace Warhammer.Rules.Tests;

public class DiceTests
{
    [Fact]
    public void SameSeedProducesSameSequence()
    {
        var a = new DeterministicRng(12345);
        var b = new DeterministicRng(12345);

        for (int i = 0; i < 1000; i++)
        {
            Assert.Equal(a.NextUInt64(), b.NextUInt64());
        }
    }

    [Fact]
    public void DifferentSeedsDiverge()
    {
        var a = new DeterministicRng(1);
        var b = new DeterministicRng(2);

        bool diverged = false;
        for (int i = 0; i < 10; i++)
        {
            if (a.NextUInt64() != b.NextUInt64())
            {
                diverged = true;
                break;
            }
        }

        Assert.True(diverged);
    }

    [Fact]
    public void CapturedStateResumesIdentically()
    {
        var rng = new DeterministicRng(999);
        for (int i = 0; i < 50; i++)
        {
            rng.Die(6);
        }

        RngState snapshot = rng.Capture();
        int[] expected = Enumerable.Range(0, 20).Select(_ => rng.Die(6)).ToArray();

        var resumed = DeterministicRng.Restore(snapshot);
        int[] actual = Enumerable.Range(0, 20).Select(_ => resumed.Die(6)).ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void D6StaysInRange()
    {
        var rng = new DeterministicRng(7);
        for (int i = 0; i < 10_000; i++)
        {
            int roll = rng.Die(6);
            Assert.InRange(roll, 1, 6);
        }
    }

    [Fact]
    public void D6IsRoughlyUniform()
    {
        var rng = new DeterministicRng(2024);
        var counts = new int[7];
        const int rolls = 60_000;

        for (int i = 0; i < rolls; i++)
        {
            counts[rng.Die(6)]++;
        }

        // Expected 10,000 each. A 5% band is loose enough never to flake and
        // tight enough to catch a genuinely biased generator.
        for (int face = 1; face <= 6; face++)
        {
            Assert.InRange(counts[face], 9_500, 10_500);
        }
    }

    [Fact]
    public void PoolCountsSuccessesAtOrAboveTarget()
    {
        var dice = new Dice(42);
        int successes = dice.Pool(count: 20, target: 4, RollKind.ToHit, "test");

        DiceRoll roll = Assert.Single(dice.Log);
        Assert.Equal(20, roll.Values.Count);
        Assert.Equal(successes, roll.Values.Count(v => v >= 4));
    }

    [Fact]
    public void PoolClampsImpossibleTargets()
    {
        var dice = new Dice(42);
        dice.Pool(count: 5, target: 9, RollKind.ToHit, "unhittable");

        // A natural 6 always hits, so a target worse than 6 is clamped rather
        // than left impossible.
        Assert.Equal(6, dice.Log[0].Target);
    }

    [Fact]
    public void LeadershipTestPassesOnEqualOrUnder()
    {
        var dice = new Dice(3);
        bool passed = dice.TestAgainst(10, RollKind.Leadership, "Ld 10 test");

        DiceRoll roll = Assert.Single(dice.Log);
        Assert.Equal(passed, roll.Total <= 10);
        Assert.Equal(passed, roll.Passed);
    }

    [Fact]
    public void EveryRollIsLogged()
    {
        var dice = new Dice(1);
        dice.D6(RollKind.Misc, "one");
        dice.TwoD6(RollKind.Misc, "two");
        dice.Pool(3, 4, RollKind.ToWound, "three");

        Assert.Equal(3, dice.Log.Count);
        Assert.Single(dice.Log[0].Values);
        Assert.Equal(2, dice.Log[1].Values.Count);
        Assert.Equal(3, dice.Log[2].Values.Count);
    }
}
