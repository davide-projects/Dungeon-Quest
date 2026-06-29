using DungeonQuest.Interfaces;

namespace DungeonQuest.Models;

public class CriticalStrikeBehavior : IAttackBehavior
{
    private static readonly Random _random = new();

    public AttackResult Execute(int baseAttack)
    {
        if (_random.Next(4) == 0)
        {
            return new AttackResult
            {
                Damage = baseAttack * 2,
                Description = "sferra un colpo critico devastante!"
            };
        }

        return new AttackResult
        {
            Damage = baseAttack,
            Description = "attacca con vigore"
        };
    }
}
