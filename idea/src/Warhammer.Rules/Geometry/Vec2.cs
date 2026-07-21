namespace Warhammer.Rules.Geometry;

/// <summary>
/// A point or direction on the table, in millimetres. Millimetres are the
/// canonical internal unit because base sizes (20mm, 25mm, 25x50mm) are the
/// awkward numbers; movement in inches converts at the API edge.
/// </summary>
public readonly record struct Vec2(double X, double Y)
{
    public static readonly Vec2 Zero = new(0, 0);

    public const double MmPerInch = 25.4;

    public static double Inches(double mm) => mm / MmPerInch;

    public static double ToMm(double inches) => inches * MmPerInch;

    public double Length => Math.Sqrt(X * X + Y * Y);

    public double LengthSquared => X * X + Y * Y;

    public Vec2 Normalised
    {
        get
        {
            double len = Length;
            return len == 0 ? Zero : new Vec2(X / len, Y / len);
        }
    }

    /// <summary>Angle in radians counter-clockwise from +X.</summary>
    public double Angle => Math.Atan2(Y, X);

    public static Vec2 FromAngle(double radians) =>
        new(Math.Cos(radians), Math.Sin(radians));

    public double Dot(Vec2 other) => X * other.X + Y * other.Y;

    public double DistanceTo(Vec2 other) => (this - other).Length;

    public Vec2 Rotated(double radians)
    {
        double c = Math.Cos(radians);
        double s = Math.Sin(radians);
        return new Vec2(X * c - Y * s, X * s + Y * c);
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator -(Vec2 v) => new(-v.X, -v.Y);

    public static Vec2 operator *(Vec2 v, double s) => new(v.X * s, v.Y * s);

    public static Vec2 operator *(double s, Vec2 v) => v * s;

    public static Vec2 operator /(Vec2 v, double s) => new(v.X / s, v.Y / s);

    public override string ToString() => $"({X:0.##}, {Y:0.##})mm";
}
