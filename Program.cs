using DungeonQuest.db;
using DungeonQuest.Models;
using DungeonQuest.UI;
using Microsoft.EntityFrameworkCore;

using var db = new DungeonContext();
db.Database.Migrate();

while (true)
{
    Console.Clear();
    GraphicsHelper.WriteMenuTitle();

    Hero hero;

    while (true)
    {
        try
        {
            var heroes = await db.Heroes
                .OrderBy(h => h.Name)
                .ToListAsync();

            if (heroes.Count == 0)
            {
                var name = Welcome.AskName();
                hero = new Hero(name);
                db.Heroes.Add(hero);
                await db.SaveChangesAsync();
                break;
            }

            Console.WriteLine();
            GraphicsHelper.WriteTitle("EROI DISPONIBILI");

            for (int i = 0; i < heroes.Count; i++)
                Console.WriteLine($"   {i + 1,2}) {heroes[i].GetShortStatus()}");

            Console.WriteLine();
            Console.WriteLine("   C) Crea un nuovo eroe");
            Console.WriteLine("   E) Elimina un eroe");
            Console.WriteLine("   0) Esci");
            Console.WriteLine();
            GraphicsHelper.WriteSeparator('-');
            Console.Write("   Scelta: ");

            var input = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (input == "0")
                return;

            if (input == "c")
            {
                var name = Welcome.AskName();
                hero = new Hero(name);
                db.Heroes.Add(hero);
                await db.SaveChangesAsync();
                break;
            }

            if (input == "e")
            {
                Console.Write("   Numero eroe da eliminare: ");
                if (!int.TryParse(Console.ReadLine()?.Trim(), out var idx) || idx < 1 || idx > heroes.Count)
                {
                    GraphicsHelper.WriteError("Numero non valido.");
                    continue;
                }

                var toDelete = heroes[idx - 1];
                Console.Write($"   Eliminare '{toDelete.Name}'? (s/N): ");
                var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (confirm != "s" && confirm != "si")
                {
                    Console.WriteLine("   Operazione annullata.");
                    continue;
                }

                db.Heroes.Remove(toDelete);
                await db.SaveChangesAsync();
                GraphicsHelper.WriteSuccess($"Eroe '{toDelete.Name}' eliminato.");
                continue;
            }

            if (int.TryParse(input, out var num) && num >= 1 && num <= heroes.Count)
            {
                hero = heroes[num - 1];

                if (hero.Hp <= 0)
                {
                    Console.Write($"   {hero.Name} è morto. Vuoi resettarlo e ricominciare? (s/N): ");
                    if (Console.ReadLine()?.Trim().ToLowerInvariant() == "s")
                    {
                        db.RemoveRange(db.Potions.Where(p => p.HeroId == hero.Id));
                        var weapons = await db.Weapons.Where(w => w.HeroId == hero.Id).ToListAsync();
                        db.RemoveRange(weapons);
                        hero.Reset();
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        continue;
                    }
                }

                await db.Entry(hero).Reference(h => h.EquippedWeapon).LoadAsync();

                break;
            }

            GraphicsHelper.WriteError("Scelta non valida.");
        }
        catch (Exception ex)
        {
            GraphicsHelper.WriteError($"Errore: {ex.Message}");
            Console.WriteLine("   Premi INVIO per riprovare...");
            Console.ReadLine();
        }
    }

    var menu = new MenuManager(db, hero);
    menu.Run();
}
