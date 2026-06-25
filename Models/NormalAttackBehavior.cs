using DungeonQuest.Interfaces;

namespace DungeonQuest.Models;

public class NormalAttackBehavior : IAttackBehavior
{
    public AttackResult Execute(int baseAttack)
    {
        return new AttackResult
        {
            Damage = baseAttack,
            Description = "attacca"
        };
    }
}
