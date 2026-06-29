namespace DungeonQuest.Models;

public class Skeleton : Enemy
{
    public Skeleton()
        : base("Scheletro", 18, 6, 10, 25, new UnreliableAttackBehavior()) { }

    public override string AsciiArt => @"
               .-.
              /ooo\
             | .-. |
             | |_| |
              `---'
               | |
              /   \";

    public override string EncounterText => "Uno Scheletro risorge dal suolo!";
}
