using DungeonQuest.Models;
using DungeonQuest.UI;

namespace DungeonQuest.Services;

public enum CombatResult
{
    Victory,
    Defeat,
    Flee
}

public class CombatManager
{
    private static readonly Random _random = new();

    public CombatResult Fight(Hero hero, Enemy enemy)
    {
        while (hero.IsAlive && enemy.IsAlive)
        {
            GraphicsHelper.WriteCombatHeader(hero.Name, enemy.Name, hero.Hp, hero.MaxHp, enemy.Hp, enemy.MaxHp);
            Console.WriteLine(GraphicsHelper.GetEnemyArt(enemy.Name));
            Console.WriteLine();
            Console.WriteLine("   1) Attacca   2) Fuggi");
            Console.Write("   Scelta: ");
            var input = Console.ReadLine()?.Trim();

            if (input == "2")
            {
                return CombatResult.Flee;
            }

            if (input != "1")
            {
                GraphicsHelper.WriteError("Scelta non valida. 1 per attaccare, 2 per fuggire.");
                Pausa();
                continue;
            }

            var heroDamage = CalculateHeroDamage(hero);
            if (heroDamage == 0)
            {
                GraphicsHelper.WriteCombatAction($"{hero.Name} tenta di colpire ma manca il bersaglio!", ConsoleColor.DarkYellow);
            }
            else
            {
                enemy.TakeDamage(heroDamage);
                GraphicsHelper.WriteCombatAction($"{hero.Name} colpisce {enemy.Name} infliggendo {heroDamage} danni!", ConsoleColor.Cyan);
            }

            if (!enemy.IsAlive)
            {
                Console.WriteLine();
                GraphicsHelper.WriteCombatAction($"{enemy.Name} sconfitto!", ConsoleColor.Green);
                return CombatResult.Victory;
            }

            var result = enemy.AttackBehavior.Execute(enemy.AttackPower);
            if (result.Damage > 0)
            {
                hero.TakeDamage(result.Damage);
                GraphicsHelper.WriteCombatAction($"{enemy.Name} {result.Description}: {hero.Name} subisce {result.Damage} danni.", ConsoleColor.Red);
            }
            else
            {
                GraphicsHelper.WriteCombatAction($"{enemy.Name} {result.Description}.", ConsoleColor.DarkYellow);
            }

            if (!hero.IsAlive)
            {
                return CombatResult.Defeat;
            }

            Pausa();
        }

        return CombatResult.Defeat;
    }

    private static void Pausa()
    {
        Console.Write("\n   Premi INVIO per continuare...");
        Console.ReadLine();
    }

    private static int CalculateHeroDamage(Hero hero)
    {
        var missChance = Math.Max(2, 20 - (hero.Level - 1) * 2);
        if (_random.Next(100) < missChance)
            return 0;

        var variation = 0.8 + _random.NextDouble() * 0.4;
        return Math.Max(1, (int)(hero.AttackPower * variation));
    }
}
