namespace Warhammer.Rules.Geometry;

/// <summary>
/// Which face of a unit something lies off. Corner cases are reported as
/// <see cref="Corner"/> rather than guessed: the rulebook resolves them with a
/// judgement call, so the rules engine surfaces the ambiguity instead of
/// silently picking a side.
/// </summary>
public enum Arc
{
    Front,
    LeftFlank,
    RightFlank,
    Rear,
    Corner,
    Inside,
}

/// <summary>
/// A unit's footprint on the table: where the centre of the block sits, and
/// which way it faces. Positions are continuous rather than grid-snapped,
/// because wheeling turns through arbitrary angles.
/// </summary>
public readonly record struct Placement(Vec2 Position, double FacingRadians, Formation Formation)
{
    private const double Epsilon = 1e-9;

    /// <summary>Unit vector the front rank looks along.</summary>
    public Vec2 Forward => Vec2.FromAngle(FacingRadians);

    /// <summary>Unit vector towards the unit's own right flank.</summary>
    public Vec2 Right => new(Math.Sin(FacingRadians), -Math.Cos(FacingRadians));

    /// <summary>
    /// Converts a world point into the unit's frame, where +Y is forward and
    /// +X is towards its right flank.
    /// </summary>
    public Vec2 ToLocal(Vec2 world)
    {
        Vec2 d = world - Position;
        return new Vec2(d.Dot(Right), d.Dot(Forward));
    }

    public Vec2 ToWorld(Vec2 local) => Position + (Right * local.X) + (Forward * local.Y);

    public Vec2 FrontLeftCorner => ToWorld(new Vec2(-Formation.HalfWidthMm, Formation.HalfDepthMm));

    public Vec2 FrontRightCorner => ToWorld(new Vec2(Formation.HalfWidthMm, Formation.HalfDepthMm));

    public Vec2 RearLeftCorner => ToWorld(new Vec2(-Formation.HalfWidthMm, -Formation.HalfDepthMm));

    public Vec2 RearRightCorner => ToWorld(new Vec2(Formation.HalfWidthMm, -Formation.HalfDepthMm));

    public IReadOnlyList<Vec2> Corners =>
        new[] { FrontLeftCorner, FrontRightCorner, RearRightCorner, RearLeftCorner };

    /// <summary>
    /// Classifies a world point by extending the four edges of this unit's
    /// footprint, per the 6th ed arc rules.
    /// </summary>
    public Arc ArcOf(Vec2 world)
    {
        Vec2 local = ToLocal(world);
        double hw = Formation.HalfWidthMm;
        double hd = Formation.HalfDepthMm;

        bool withinWidth = Math.Abs(local.X) <= hw + Epsilon;
        bool withinDepth = Math.Abs(local.Y) <= hd + Epsilon;

        if (withinWidth && withinDepth)
        {
            return Arc.Inside;
        }

        if (withinWidth)
        {
            return local.Y > 0 ? Arc.Front : Arc.Rear;
        }

        if (withinDepth)
        {
            return local.X > 0 ? Arc.RightFlank : Arc.LeftFlank;
        }

        return Arc.Corner;
    }

    /// <summary>
    /// Classifies another unit by the arc its centre lies in. Deliberately
    /// simple for now — the rulebook cares about where the charging unit
    /// actually makes contact, which needs the charge path, not just a centre
    /// point. Revisit when charges are implemented.
    /// </summary>
    public Arc ArcOf(Placement other) => ArcOf(other.Position);

    /// <summary>
    /// Wheels the unit about one of its front corners, as in the movement
    /// phase. Returns the new placement and the movement cost, which is the
    /// arc length travelled by the outer front corner.
    /// </summary>
    /// <param name="radians">
    /// Positive wheels to the unit's right (pivoting on the front-right
    /// corner); negative wheels left.
    /// </param>
    public (Placement Result, double CostMm) Wheel(double radians)
    {
        if (radians == 0)
        {
            return (this, 0);
        }

        Vec2 pivot = radians > 0 ? FrontRightCorner : FrontLeftCorner;

        // Wheeling right turns the facing clockwise, which is negative in a
        // counter-clockwise angle convention.
        double turn = -radians;

        Vec2 newPosition = pivot + (Position - pivot).Rotated(turn);
        var result = this with
        {
            Position = newPosition,
            FacingRadians = NormaliseAngle(FacingRadians + turn),
        };

        double cost = Formation.WidthMm * Math.Abs(radians);
        return (result, cost);
    }

    /// <summary>Advances straight ahead. Distance is in millimetres.</summary>
    public Placement Advance(double distanceMm) => this with { Position = Position + (Forward * distanceMm) };

    public static double NormaliseAngle(double radians)
    {
        double twoPi = Math.PI * 2;
        double a = radians % twoPi;
        if (a <= -Math.PI)
        {
            a += twoPi;
        }
        else if (a > Math.PI)
        {
            a -= twoPi;
        }

        return a;
    }

    public override string ToString() =>
        $"{Formation} at {Position} facing {FacingRadians * 180 / Math.PI:0.#}deg";
}
