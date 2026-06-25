using DungeonQuest.Interfaces;

namespace DungeonQuest.Models;

public class UnreliableAttackBehavior : IAttackBehavior
{
    private static readonly Random _random = new();

    public AttackResult Execute(int baseAttack)
    {
        if (_random.Next(2) == 0)
        {
            return new AttackResult
            {
                Damage = 0,
                Description = "tenta di colpire ma manca il bersaglio"
            };
        }

        return new AttackResult
        {
            Damage = baseAttack,
            Description = "colpisce"
        };
    }
}
