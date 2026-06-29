using DungeonQuest.Interfaces;

namespace DungeonQuest.Models;

public class DarkLordAttackBehavior : IAttackBehavior
{
    private static readonly Random _random = new();

    public AttackResult Execute(int baseAttack)
    {
        int roll = _random.Next(100);

        if (roll < 4)
        {
            return new AttackResult
            {
                Damage = 0,
                Description = "scatena un'onda d'oscurità ma l'eroe riesce a schivarla"
            };
        }

        if (roll < 74)
        {
            return new AttackResult
            {
                Damage = baseAttack * 2,
                Description = "sferra un colpo terrificante con il suo mace nero!"
            };
        }

        return new AttackResult
        {
            Damage = baseAttack,
            Description = "attacca con furore oscuro"
        };
    }
}
