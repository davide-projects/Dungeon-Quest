namespace DungeonQuest.Models;

public class Potion
{
    public int Id { get; set; }

    public string Name { get; private set; }

    public int? HeroId { get; set; }
    public Hero? Hero { get; set; }

    public Potion(string name)
    {
        Name = name;
    }

    private Potion() { }

    public override string ToString()
    {
        return $"{Name} (cura il 50% della salute massima)";
    }
}
