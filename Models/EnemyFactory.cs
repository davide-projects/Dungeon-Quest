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
        new(() => new Goblin(), 1, _ => 40),
        new(() => new Orc(), 1, _ => 30),
        new(() => new UrukHai(), 4, _ => 20),
    ];

    public static Enemy Generate(int heroLevel)
    {
        var available = Entries.Where(e => heroLevel >= e.MinLevel).ToList();
        var weights = available.Select(e => Math.Max(0, e.WeightFunc(heroLevel))).ToList();
        double totalWeight = weights.Sum();

        if (totalWeight <= 0)
        {
            var enemy = available[0].Create();
            enemy.ScaleToLevel(heroLevel);
            return enemy;
        }

        double roll = Random.NextDouble() * totalWeight;
        double cumulative = 0;

        for (int i = 0; i < available.Count; i++)
        {
            cumulative += weights[i];
            if (roll < cumulative)
            {
                var enemy = available[i].Create();
                enemy.ScaleToLevel(heroLevel);
                return enemy;
            }
        }

        var lastEnemy = available[^1].Create();
        lastEnemy.ScaleToLevel(heroLevel);
        return lastEnemy;
    }
}
