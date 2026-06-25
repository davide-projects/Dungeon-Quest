namespace DungeonQuest.Models;

public class Skeleton : Enemy
{
    public Skeleton()
        : base("Scheletro", 18, 6, 10, 25, new UnreliableAttackBehavior()) { }
}
