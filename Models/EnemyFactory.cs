namespace DungeonQuest.Models;

public class EnemyEntry
{
    public Func<Enemy> Create { get; }
    public int MinLevel { get; }
    public Func<int, double> WeightFunc { get; }

    public EnemyEntry(Func<Enemy> create, int minLevel, Func<int, double> weightFunc)
    {
        Create = create;
        MinLevel = minLevel;
        WeightFunc = weightFunc;
    }
}

public static class EnemyFactory
{
    private static readonly Random Random = new();
    private static readonly List<EnemyEntry> Entries =
    [
        new(() => new Goblin(), 1, l => l < 2 ? 50 : (100.0 - Math.Min(45, (l - 1) * 5)) / 2),
        new(() => new Skeleton(), 1, l => l < 2 ? 50 : (100.0 - Math.Min(45, (l - 1) * 5)) / 2),
        new(() => new Dragon(), 2, l => Math.Min(45, (l - 1) * 5)),
    ];

    public static Enemy Generate(int heroLevel)
    {
        var available = Entries.Where(e => heroLevel >= e.MinLevel).ToList();
        var weights = available.Select(e => Math.Max(0, e.WeightFunc(heroLevel))).ToList();
        double totalWeight = weights.Sum();

        if (totalWeight <= 0)
            return available[0].Create();

        double roll = Random.NextDouble() * totalWeight;
        double cumulative = 0;

        for (int i = 0; i < available.Count; i++)
        {
            cumulative += weights[i];
            if (roll < cumulative)
                return available[i].Create();
        }

        return available[^1].Create();
    }
}
