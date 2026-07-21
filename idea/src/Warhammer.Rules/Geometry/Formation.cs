namespace Warhammer.Rules.Geometry;

/// <summary>Base dimensions in millimetres, as cast.</summary>
public readonly record struct BaseSize(double WidthMm, double DepthMm)
{
    public static readonly BaseSize Infantry20 = new(20, 20);
    public static readonly BaseSize Infantry25 = new(25, 25);
    public static readonly BaseSize Cavalry = new(25, 50);
    public static readonly BaseSize Monster40 = new(40, 40);
    public static readonly BaseSize Monster50 = new(50, 50);
}

/// <summary>
/// A block of models: how many, how wide, and what they stand on. Ranks are
/// derived rather than stored, so a unit cannot get into a state where its
/// model count and its formation disagree.
/// </summary>
public readonly record struct Formation(int ModelCount, int Frontage, BaseSize BaseSize)
{
    /// <summary>
    /// Validating factory. A positional record leaves <c>with</c> expressions
    /// able to sidestep this, so the derived members guard themselves too.
    /// </summary>
    public static Formation Create(int modelCount, int frontage, BaseSize baseSize)
    {
        if (modelCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(modelCount), "Model count cannot be negative.");
        }

        if (frontage <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(frontage), "Frontage must be positive.");
        }

        return new Formation(modelCount, frontage, baseSize);
    }

    private int SafeFrontage => Frontage > 0
        ? Frontage
        : throw new InvalidOperationException($"Formation frontage must be positive, was {Frontage}.");

    /// <summary>Ranks including a partial back rank.</summary>
    public int Ranks => ModelCount == 0 ? 0 : (int)Math.Ceiling(ModelCount / (double)SafeFrontage);

    /// <summary>Ranks that are completely filled.</summary>
    public int FullRanks => ModelCount / SafeFrontage;

    public int ModelsInBackRank
    {
        get
        {
            if (ModelCount == 0)
            {
                return 0;
            }

            int remainder = ModelCount % SafeFrontage;
            return remainder == 0 ? SafeFrontage : remainder;
        }
    }

    public double WidthMm => SafeFrontage * BaseSize.WidthMm;

    public double DepthMm => Ranks * BaseSize.DepthMm;

    public double HalfWidthMm => WidthMm / 2.0;

    public double HalfDepthMm => DepthMm / 2.0;

    /// <summary>
    /// Rank bonus for combat resolution: +1 per full rank behind the first,
    /// where a rank counts only if it holds at least <see cref="MinModelsPerRank"/>
    /// models, capped at <see cref="MaxRankBonus"/>.
    /// </summary>
    /// <remarks>
    /// Both constants need checking against the 6th ed rulebook — see
    /// docs/rules-to-verify.md.
    /// </remarks>
    public int RankBonus
    {
        get
        {
            if (Frontage < MinModelsPerRank)
            {
                return 0;
            }

            int bonusRanks = FullRanks - 1;
            return Math.Clamp(bonusRanks, 0, MaxRankBonus);
        }
    }

    public const int MinModelsPerRank = 5;
    public const int MaxRankBonus = 3;

    /// <summary>Removes casualties from the back rank, keeping frontage intact.</summary>
    public Formation WithCasualties(int wounds)
    {
        int remaining = Math.Max(0, ModelCount - wounds);
        int frontage = Math.Min(Frontage, Math.Max(1, remaining));
        return this with { ModelCount = remaining, Frontage = frontage };
    }

    public override string ToString() =>
        $"{ModelCount} models, {Frontage} wide x {Ranks} deep ({WidthMm:0}x{DepthMm:0}mm)";
}
