using Warhammer.Rules.Geometry;
using Xunit;

namespace Warhammer.Rules.Tests;

public class FormationTests
{
    [Theory]
    [InlineData(20, 5, 4, 4, 5)]
    [InlineData(17, 5, 4, 3, 2)]
    [InlineData(5, 5, 1, 1, 5)]
    [InlineData(1, 5, 1, 0, 1)]
    public void RanksAreDerivedFromModelCount(
        int models, int frontage, int expectedRanks, int expectedFullRanks, int expectedBackRank)
    {
        var formation = new Formation(models, frontage, BaseSize.Infantry20);

        Assert.Equal(expectedRanks, formation.Ranks);
        Assert.Equal(expectedFullRanks, formation.FullRanks);
        Assert.Equal(expectedBackRank, formation.ModelsInBackRank);
    }

    [Fact]
    public void FootprintIsFrontageByRanks()
    {
        var formation = new Formation(20, 5, BaseSize.Infantry20);

        Assert.Equal(100, formation.WidthMm);
        Assert.Equal(80, formation.DepthMm);
    }

    [Theory]
    [InlineData(20, 5, 3)]  // 4 full ranks, capped at +3
    [InlineData(17, 5, 2)]  // 3 full ranks
    [InlineData(10, 5, 1)]
    [InlineData(5, 5, 0)]   // single rank
    [InlineData(12, 4, 0)]  // too narrow to score ranks
    public void RankBonusRespectsWidthAndCap(int models, int frontage, int expected)
    {
        var formation = new Formation(models, frontage, BaseSize.Infantry20);
        Assert.Equal(expected, formation.RankBonus);
    }

    [Fact]
    public void CasualtiesComeOffTheBackRank()
    {
        var formation = new Formation(20, 5, BaseSize.Infantry20).WithCasualties(3);

        Assert.Equal(17, formation.ModelCount);
        Assert.Equal(5, formation.Frontage);
        Assert.Equal(4, formation.Ranks);
    }

    [Fact]
    public void FrontageNarrowsWhenTheUnitIsNearlyWipedOut()
    {
        var formation = new Formation(20, 5, BaseSize.Infantry20).WithCasualties(18);

        Assert.Equal(2, formation.ModelCount);
        Assert.Equal(2, formation.Frontage);
    }
}

public class ArcTests
{
    // 20 models, 5 wide, 20mm bases: a 100mm x 80mm block at the origin
    // facing +Y (north).
    private static Placement NorthFacingBlock() =>
        new(Vec2.Zero, Math.PI / 2, new Formation(20, 5, BaseSize.Infantry20));

    [Theory]
    [InlineData(0, 200, Arc.Front)]
    [InlineData(0, -200, Arc.Rear)]
    [InlineData(200, 0, Arc.RightFlank)]
    [InlineData(-200, 0, Arc.LeftFlank)]
    [InlineData(200, 200, Arc.Corner)]
    [InlineData(-200, -200, Arc.Corner)]
    [InlineData(0, 0, Arc.Inside)]
    public void ArcsAreBoundedByTheExtendedEdges(double x, double y, Arc expected)
    {
        Assert.Equal(expected, NorthFacingBlock().ArcOf(new Vec2(x, y)));
    }

    [Fact]
    public void ArcsFollowFacing()
    {
        // Same block turned to face +X (east): what was its right flank is now
        // its rear.
        var block = NorthFacingBlock() with { FacingRadians = 0 };

        Assert.Equal(Arc.Front, block.ArcOf(new Vec2(200, 0)));
        Assert.Equal(Arc.Rear, block.ArcOf(new Vec2(-200, 0)));
        Assert.Equal(Arc.RightFlank, block.ArcOf(new Vec2(0, -200)));
        Assert.Equal(Arc.LeftFlank, block.ArcOf(new Vec2(0, 200)));
    }

    [Fact]
    public void CornersSitAtTheFootprintEdges()
    {
        Placement block = NorthFacingBlock();

        AssertClose(new Vec2(-50, 40), block.FrontLeftCorner);
        AssertClose(new Vec2(50, 40), block.FrontRightCorner);
        AssertClose(new Vec2(50, -40), block.RearRightCorner);
        AssertClose(new Vec2(-50, -40), block.RearLeftCorner);
    }

    private static void AssertClose(Vec2 expected, Vec2 actual, double tolerance = 1e-6)
    {
        Assert.True(
            expected.DistanceTo(actual) < tolerance,
            $"Expected {expected} but got {actual}");
    }
}

public class WheelTests
{
    private static Placement NorthFacingBlock() =>
        new(Vec2.Zero, Math.PI / 2, new Formation(20, 5, BaseSize.Infantry20));

    [Fact]
    public void WheelingRightPivotsOnTheFrontRightCorner()
    {
        Placement block = NorthFacingBlock();
        Vec2 pivot = block.FrontRightCorner;

        (Placement result, _) = block.Wheel(Math.PI / 2);

        // The block turns rigidly about that corner, so it stays put and stays
        // the front-right corner. Facing north, wheeling right, ends up east.
        Assert.True(pivot.DistanceTo(result.FrontRightCorner) < 1e-6);
        Assert.Equal(0, result.FacingRadians, 6);
    }

    [Fact]
    public void WheelingLeftPivotsOnTheFrontLeftCorner()
    {
        Placement block = NorthFacingBlock();
        Vec2 pivot = block.FrontLeftCorner;

        (Placement result, _) = block.Wheel(-Math.PI / 2);

        // Facing north, wheeling left, ends up west.
        Assert.True(pivot.DistanceTo(result.FrontLeftCorner) < 1e-6);
        Assert.Equal(Math.PI, result.FacingRadians, 6);
    }

    [Fact]
    public void WheelCostIsTheOuterCornersArcLength()
    {
        Placement block = NorthFacingBlock();
        (_, double cost) = block.Wheel(Math.PI / 2);

        // A 100mm frontage wheeling a quarter turn: 100 * pi/2 = 157.08mm,
        // which is 6.18" — most of an infantry unit's move.
        Assert.Equal(100 * Math.PI / 2, cost, 6);
        Assert.Equal(6.18, Vec2.Inches(cost), 2);
    }

    [Fact]
    public void WheelingIsFreeWhenTheAngleIsZero()
    {
        Placement block = NorthFacingBlock();
        (Placement result, double cost) = block.Wheel(0);

        Assert.Equal(0, cost);
        Assert.Equal(block, result);
    }

    [Fact]
    public void AdvanceMovesAlongFacing()
    {
        Placement block = NorthFacingBlock().Advance(Vec2.ToMm(4));

        Assert.Equal(0, block.Position.X, 6);
        Assert.Equal(101.6, block.Position.Y, 6);
    }
}
