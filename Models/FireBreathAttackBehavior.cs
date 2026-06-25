using DungeonQuest.Interfaces;

namespace DungeonQuest.Models;

public class FireBreathAttackBehavior : IAttackBehavior
{
    private static readonly Random _random = new();

    public AttackResult Execute(int baseAttack)
    {
        if (_random.Next(4) == 0)
        {
            return new AttackResult
            {
                Damage = baseAttack * 2,
                Description = "sferra un soffio infuocato infliggendo danno doppio"
            };
        }

        return new AttackResult
        {
            Damage = baseAttack,
            Description = "attacca"
        };
    }
}
