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

    Hero eroe;

    while (true)
    {
        try
        {
            var eroi = await db.Heroes
                .OrderBy(h => h.Name)
                .ToListAsync();

            if (eroi.Count == 0)
            {
                var nome = Benvenuto.ChiediNome();
                eroe = new Hero(nome);
                db.Heroes.Add(eroe);
                await db.SaveChangesAsync();
                break;
            }

            Console.WriteLine();
            GraphicsHelper.WriteTitle("EROI DISPONIBILI");

            for (int i = 0; i < eroi.Count; i++)
                Console.WriteLine($"   {i + 1,2}) {eroi[i].GetShortStatus()}");

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
                var nome = Benvenuto.ChiediNome();
                eroe = new Hero(nome);
                db.Heroes.Add(eroe);
                await db.SaveChangesAsync();
                break;
            }

            if (input == "e")
            {
                Console.Write("   Numero eroe da eliminare: ");
                if (!int.TryParse(Console.ReadLine()?.Trim(), out var idx) || idx < 1 || idx > eroi.Count)
                {
                    GraphicsHelper.WriteError("Numero non valido.");
                    continue;
                }

                var daEliminare = eroi[idx - 1];
                Console.Write($"   Eliminare '{daEliminare.Name}'? (s/N): ");
                var conferma = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (conferma != "s" && conferma != "si")
                {
                    Console.WriteLine("   Operazione annullata.");
                    continue;
                }

                db.Heroes.Remove(daEliminare);
                await db.SaveChangesAsync();
                GraphicsHelper.WriteSuccess($"Eroe '{daEliminare.Name}' eliminato.");
                continue;
            }

            if (int.TryParse(input, out var num) && num >= 1 && num <= eroi.Count)
            {
                eroe = eroi[num - 1];

                if (eroe.Hp <= 0)
                {
                    Console.Write($"   {eroe.Name} è morto. Vuoi resettarlo e ricominciare? (s/N): ");
                    if (Console.ReadLine()?.Trim().ToLowerInvariant() == "s")
                    {
                        db.RemoveRange(db.Potions.Where(p => p.HeroId == eroe.Id));
                        var armi = await db.Weapons.Where(w => w.HeroId == eroe.Id).ToListAsync();
                        db.RemoveRange(armi);
                        eroe.Reset();
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        continue;
                    }
                }

                await db.Entry(eroe).Reference(h => h.EquippedWeapon).LoadAsync();

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

    var menu = new MenuManager(db, eroe);
    menu.Run();
}
