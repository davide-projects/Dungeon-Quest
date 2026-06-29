namespace DungeonQuest.Models;

public enum PotionKind
{
    Healing,
    XpBoost
}

public class Potion
{
    public int Id { get; set; }

    public string Name { get; private set; }

    public PotionKind Kind { get; private set; }

    public int? HeroId { get; set; }
    public Hero? Hero { get; set; }

    public Potion(string name, PotionKind kind = PotionKind.Healing)
    {
        Name = name;
        Kind = kind;
    }

    private Potion() { Name = null!; }

    public override string ToString()
    {
        return Kind switch
        {
            PotionKind.XpBoost => $"{Name} (moltiplica XP del prossimo combattimento x2)",
            _ => $"{Name} (cura il 50% della salute massima)"
        };
    }
}
