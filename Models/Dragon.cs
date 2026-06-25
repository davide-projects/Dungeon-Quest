namespace DungeonQuest.Models;

public class Dragon : Enemy
{
    public Dragon()
        : base("Drago", 40, 10, 30, 60, new FireBreathAttackBehavior()) { }
}
