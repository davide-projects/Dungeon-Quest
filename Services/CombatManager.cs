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
    private const int BaseMissChance = 20;
    private const int MissReductionPerLevel = 2;
    private const int MinMissChance = 2;
    private const double MinDamageMultiplier = 0.8;
    private const double MaxDamageVariation = 0.4;

    private static readonly Random _random = new();
    private readonly ArsenalManager _arsenal;

    public CombatManager(ArsenalManager arsenal)
    {
        _arsenal = arsenal;
    }

    public CombatResult Fight(Hero hero, Enemy enemy)
    {
        while (hero.IsAlive && enemy.IsAlive)
        {
            GraphicsHelper.WriteCombatHeader(hero.Name, enemy.Name, hero.Hp, hero.MaxHp, enemy.Hp, enemy.MaxHp);
            Console.WriteLine(GraphicsHelper.GetEnemyArt(enemy.Name));
            Console.WriteLine();

            var input = ReadChoice(hero);

            if (input == "2")
                return CombatResult.Flee;

            if (input == "3")
            {
                _arsenal.TryUsePotion();
                GraphicsHelper.Pause();
                continue;
            }

            if (input != "1")
            {
                GraphicsHelper.WriteError("Scelta non valida.");
                GraphicsHelper.Pause();
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
                return CombatResult.Defeat;

            GraphicsHelper.Pause();
        }

        return CombatResult.Defeat;
    }

    private string ReadChoice(Hero hero)
    {
        if (_arsenal.PotionCount > 0)
            Console.WriteLine($"   1) Attacca   2) Fuggi   3) Usa pozione ({_arsenal.PotionCount})");
        else
            Console.WriteLine("   1) Attacca   2) Fuggi");

        Console.Write("   Scelta: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    private static int CalculateHeroDamage(Hero hero)
    {
        var missChance = Math.Max(MinMissChance, BaseMissChance - (hero.Level - 1) * MissReductionPerLevel);
        if (_random.Next(100) < missChance)
            return 0;

        var variation = MinDamageMultiplier + _random.NextDouble() * MaxDamageVariation;
        return Math.Max(1, (int)(hero.AttackPower * variation));
    }
}
