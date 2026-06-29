using DungeonQuest.db;
using DungeonQuest.Exceptions;
using DungeonQuest.Interfaces;
using DungeonQuest.Models;
using DungeonQuest.Services;

namespace DungeonQuest.UI;

public class MenuManager
{
    private readonly DungeonContext _db;
    private readonly Hero _hero;
    private readonly ArsenalManager _arsenal;
    private readonly CombatManager _combat;
    private readonly ShopManager _shop;
    private readonly List<IExporter> _exporters;

    public MenuManager(DungeonContext db, Hero hero)
    {
        _db = db;
        _hero = hero;
        _arsenal = new ArsenalManager(db, hero);
        _combat = new CombatManager(_arsenal);
        _shop = new ShopManager(_arsenal, hero);
        _exporters = [new CsvExporter(), new JsonExporter()];
    }

    public void Run()
    {
        int choice = -1;
        do
        {
            try { Console.Clear(); } catch (IOException) { }
            GraphicsHelper.WriteMenuTitle();
            GraphicsHelper.WriteSeparator('-');
            Console.Write("  ");
            GraphicsHelper.WriteHeroStatus(_hero);
            GraphicsHelper.WriteSeparator('-');
            Console.WriteLine();

            bool canClaim = _hero.Level == 1 && _arsenal.GetAllWeapons().Count == 0;
            if (canClaim)
            {
                GraphicsHelper.WriteLineColor("   R) Raccogli la tua prima arma!", ConsoleColor.Yellow);
                Console.WriteLine();
            }

            bool canBoss = _hero.Level >= 10;
            if (canBoss)
            {
                GraphicsHelper.WriteLineColor("   B) AFFRONTA IL BOSS FINALE!", ConsoleColor.Red);
                Console.WriteLine();
            }

            Console.WriteLine("   1) Mostra inventario");
            Console.WriteLine("   2) Mostra l'arsenale per tipo");
            Console.WriteLine("   3) Cerca un'arma ed equipaggiala");
            Console.WriteLine("   4) Negozio");
            Console.WriteLine("   5) Combatti contro un nemico");
            Console.WriteLine("   6) Esporta l'arsenale su file");
            Console.WriteLine("   0) Torna al menu principale");
            Console.WriteLine();
            GraphicsHelper.WriteSeparator('-');
            Console.Write("   Scelta: ");

            var input = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "";

            if (canClaim && input == "r")
            {
                ClaimStarterWeapon();
                GraphicsHelper.Pause();
                continue;
            }

            if (canBoss && input == "b")
            {
                BossFight();
                if (_hero.IsAlive)
                    GraphicsHelper.Pause();
                continue;
            }

            if (!int.TryParse(input, out choice))
            {
                GraphicsHelper.WriteError("Input non valido. Inserisci un numero.");
                GraphicsHelper.Pause();
                choice = -1;
                continue;
            }

            try
            {
                HandleChoice(choice);
            }
            catch (ExportException ex)
            {
                GraphicsHelper.WriteError(ex.Message);
            }
            catch (WeaponNotFoundException ex)
            {
                GraphicsHelper.WriteError(ex.Message);
            }
            catch (InvalidWeaponException ex)
            {
                GraphicsHelper.WriteError(ex.Message);
            }
            catch (Exception ex)
            {
                GraphicsHelper.WriteError($"Errore imprevisto: {ex.Message}");
            }

            if (choice != 0)
                GraphicsHelper.Pause();

        } while (choice != 0);

        _db.SaveChanges();
        Console.WriteLine("Ritorno al menu principale...");
    }

    private void HandleChoice(int choice)
    {
        switch (choice)
        {
            case 1:
                ShowAll();
                break;
            case 2:
                ShowByType();
                break;
            case 3:
                SearchAndEquip();
                break;
            case 4:
                Shop();
                break;
            case 5:
                Fight();
                break;
            case 6:
                ExportArsenal();
                break;
            case 0:
                break;
            default:
                Console.WriteLine("Scelta non valida.");
                break;
        }
    }

    private void ClaimStarterWeapon()
    {
        var template = _shop.GetRandomCommon();
        var weapon = new Weapon(template.Name, template.Type, template.Damage, template.Rarity);
        _arsenal.AddWeapon(weapon);
        _hero.EquippedWeapon = weapon;
        _db.SaveChanges();

        Console.WriteLine();
        GraphicsHelper.WriteTitle("PRIMA ARMA!");
        Console.Write("  Hai raccolto: ");
        GraphicsHelper.WriteWeapon(weapon);
        Console.WriteLine("  E' stata equipaggiata automaticamente.");
    }

    private void Shop()
    {
        var catalog = _shop.GetCatalog();
        var buyable = catalog.Where(c => !c.Owned).ToList();

        if (buyable.Count == 0)
        {
            GraphicsHelper.WriteError("Hai gia' tutte le armi disponibili!");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle("NEGOZIO");
        GraphicsHelper.WriteColor($"  Oro disponibile: ", ConsoleColor.White);
        GraphicsHelper.WriteLineColor($"{_hero.Gold}", ConsoleColor.Yellow);
        Console.WriteLine();

        for (int i = 0; i < buyable.Count; i++)
        {
            var (template, _) = buyable[i];
            Console.Write($"   {i + 1,2}) ");
            GraphicsHelper.WriteWeapon(template.Name, template.Type, template.Damage, template.Rarity);
            Console.Write("       Prezzo: ");
            GraphicsHelper.WriteColor($"{template.Price} oro", ConsoleColor.Yellow);
            Console.Write(" | ");
            if (_shop.CanAfford(template))
                GraphicsHelper.WriteColor("ACQUISTABILE", ConsoleColor.Green);
            else
                GraphicsHelper.WriteColor("NON DISPONIBILE", ConsoleColor.Red);
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.Write("   Scegli arma da acquistare (0 per annullare): ");

        if (!int.TryParse(Console.ReadLine()?.Trim(), out var choice) || choice < 0 || choice > buyable.Count)
        {
            GraphicsHelper.WriteError("Scelta non valida.");
            return;
        }

        if (choice == 0)
        {
            Console.WriteLine("   Operazione annullata.");
            return;
        }

        var selected = buyable[choice - 1].Template;

        if (!_shop.TryBuy(selected))
        {
            GraphicsHelper.WriteError("Oro insufficiente!");
            return;
        }

        Console.Write("   >> ");
        GraphicsHelper.WriteColor("Acquistata: ", ConsoleColor.Green);
        GraphicsHelper.WriteWeapon(selected.Name, selected.Type, selected.Damage, selected.Rarity);
        GraphicsHelper.WriteColor($"   Oro rimasto: {_hero.Gold}", ConsoleColor.Yellow);
        Console.WriteLine();
    }

    private void ShowAll()
    {
        var weapons = _arsenal.GetAllWeapons();
        var potions = _arsenal.Potions;
        bool hasWeapons = weapons.Count > 0;
        bool hasPotions = potions.Count > 0;

        if (!hasWeapons && !hasPotions)
        {
            GraphicsHelper.WriteError("Nessun oggetto nell'inventario.");
            return;
        }

        Console.WriteLine();

        if (hasPotions)
        {
            GraphicsHelper.WriteTitle($"POZIONI ({potions.Count})");
            foreach (var potion in potions)
                GraphicsHelper.WritePotion(potion, "  * ");
            Console.WriteLine();
        }

        if (_hero.XpBoostRemaining > 0)
        {
            GraphicsHelper.WriteColor("  Potenziamento XP: ", ConsoleColor.Cyan);
            GraphicsHelper.WriteColor($"{_hero.XpBoostRemaining} combattimento(i) rimasto(i)", ConsoleColor.Cyan);
            Console.WriteLine();
        }

        if (hasWeapons)
            GraphicsHelper.WriteItemList($"ARMI ({weapons.Count})", weapons);
    }

    private void ShowByType()
    {
        Console.WriteLine();
        GraphicsHelper.WriteTitle("TIPO ARMA");
        Console.WriteLine("   " + GraphicsHelper.GetWeaponTypeMenu());
        Console.Write("   Inserisci tipo: ");
        var typeInput = Console.ReadLine()?.Trim();
        var typeChoice = GraphicsHelper.ParseWeaponTypeChoice(typeInput ?? "");
        if (typeChoice < 0)
        {
            GraphicsHelper.WriteError("Tipo non valido.");
            return;
        }

        var type = WeaponType.All[typeChoice - 1];
        var weapons = _arsenal.GetWeaponsByType(type);

        if (weapons.Count == 0)
        {
            GraphicsHelper.WriteError($"Nessuna arma di tipo {type} nell'arsenale.");
            return;
        }

        GraphicsHelper.WriteItemList(type.ToString(), weapons);
    }

    private void SearchAndEquip()
    {
        var weapons = _arsenal.GetAllWeapons();

        if (weapons.Count == 0)
        {
            GraphicsHelper.WriteError("Nessuna arma nell'arsenale.");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle("CAMBIA ARMA");
        GraphicsHelper.WriteLineColor($"   Arma equipaggiata: {_hero.EquippedWeapon?.Name ?? "a mani nude"}", ConsoleColor.Cyan);
        Console.WriteLine();

        for (int i = 0; i < weapons.Count; i++)
        {
            string marker = weapons[i].Id == _hero.EquippedWeapon?.Id ? "\u2190 equipaggiata" : "";
            GraphicsHelper.WriteWeapon(weapons[i], $"   {i + 1}) ");
            if (!string.IsNullOrEmpty(marker))
                GraphicsHelper.WriteLineColor(marker, ConsoleColor.Green);
        }

        Console.WriteLine();
        Console.Write("   Scegli un'arma da equipaggiare (0 per annullare): ");

        if (!int.TryParse(Console.ReadLine()?.Trim(), out var choice) || choice < 0 || choice > weapons.Count)
        {
            GraphicsHelper.WriteError("Scelta non valida.");
            return;
        }

        if (choice == 0)
        {
            Console.WriteLine("   Operazione annullata.");
            return;
        }

        var selected = weapons[choice - 1];

        if (selected.Id == _hero.EquippedWeapon?.Id)
        {
            GraphicsHelper.WriteCombatAction("Arma gia' equipaggiata.", ConsoleColor.DarkYellow);
            return;
        }

        _hero.EquippedWeapon = selected;
        Console.Write("   >> ");
        GraphicsHelper.WriteColor("Equipaggiata: ", ConsoleColor.Green);
        GraphicsHelper.WriteWeapon(selected);
    }

    private void Fight()
    {
        if (!_hero.IsAlive)
        {
            GraphicsHelper.WriteError("L'eroe è a terra! Non può combattere.");
            return;
        }

        var enemy = EnemyFactory.Generate(_hero.Level);
        GraphicsHelper.WriteTitle("INCONTRO!");
        Console.WriteLine(enemy.AsciiArt);
        Console.WriteLine();
        GraphicsHelper.WriteLineColor("  " + enemy.EncounterText, ConsoleColor.Magenta);
        GraphicsHelper.Pause();

        var outcome = _combat.Fight(_hero, enemy);

        switch (outcome)
        {
            case CombatResult.Victory:
                int xpReward = enemy.XpReward;
                if (_hero.XpBoostRemaining > 0)
                {
                    xpReward *= 2;
                    _hero.XpBoostRemaining--;
                    GraphicsHelper.WriteCombatAction("Bonus XP attivo! XP raddoppiati!", ConsoleColor.Cyan);
                }
                GraphicsHelper.WriteVictory(enemy.Name, enemy.GoldReward, xpReward);
                _hero.AddReward(enemy.GoldReward, xpReward);
                _db.SaveChanges();

                string healMsg = enemy.OnDefeated(_hero);
                if (!string.IsNullOrEmpty(healMsg))
                {
                    GraphicsHelper.WriteCombatAction(healMsg, ConsoleColor.Green);
                    _db.SaveChanges();
                }

                if (_hero.LevelUpMessage is not null)
                {
                    GraphicsHelper.WriteCombatAction(_hero.LevelUpMessage, ConsoleColor.Green);
                    _hero.LevelUpMessage = null;
                }

                if (enemy.TryDropPotion())
                {
                    GraphicsHelper.WriteSuccess("Hai ottenuto una Pozione curativa!");
                    _arsenal.AddPotion(new Potion("Pozione curativa"));
                }

                if (enemy.TryDropXpBoostPotion())
                {
                    _hero.XpBoostRemaining++;
                    GraphicsHelper.WriteSuccess("Potenziamento XP attivo! Il prossimo combattimento darà XP doppi!");
                }
                break;
            case CombatResult.Defeat:
                GraphicsHelper.WriteDefeat();
                break;
            case CombatResult.Flee:
                GraphicsHelper.WriteFlee();
                break;
        }

        Console.WriteLine();
        Console.Write("  ");
        GraphicsHelper.WriteHeroStatus(_hero);
    }

    private void BossFight()
    {
        if (!_hero.IsAlive)
        {
            GraphicsHelper.WriteError("L'eroe è a terra! Non può combattere.");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle("BOSS FINALE");
        GraphicsHelper.WriteColor("  Sei sicuro di voler affrontare Sauron?", ConsoleColor.Red);
        Console.WriteLine();
        Console.Write("   (s/N): ");
        if (Console.ReadLine()?.Trim().ToLowerInvariant() != "s")
        {
            Console.WriteLine("   Saggio... forse un giorno sarai pronto.");
            return;
        }

        var boss = new Sauron();
        GraphicsHelper.WriteTitle("SAURON");
        Console.WriteLine(boss.AsciiArt);
        Console.WriteLine();
        GraphicsHelper.WriteLineColor("  " + boss.EncounterText, ConsoleColor.Red);
        GraphicsHelper.Pause();

        var outcome = _combat.Fight(_hero, boss);

        switch (outcome)
        {
            case CombatResult.Victory:
                int xpReward = boss.XpReward;
                if (_hero.XpBoostRemaining > 0)
                {
                    xpReward *= 2;
                    _hero.XpBoostRemaining--;
                    GraphicsHelper.WriteCombatAction("Bonus XP attivo! XP raddoppiati!", ConsoleColor.Cyan);
                }
                _hero.AddReward(boss.GoldReward, xpReward);
                _db.SaveChanges();
                string healMsg = boss.OnDefeated(_hero);
                if (!string.IsNullOrEmpty(healMsg))
                    _db.SaveChanges();
                GraphicsHelper.WriteBossVictory(boss.Name, boss.GoldReward, xpReward);
                Environment.Exit(0);
                break;
            case CombatResult.Defeat:
                GraphicsHelper.WriteDefeat();
                break;
            case CombatResult.Flee:
                GraphicsHelper.WriteFlee();
                break;
        }

        Console.WriteLine();
        Console.Write("  ");
        GraphicsHelper.WriteHeroStatus(_hero);
    }

    private void ExportArsenal()
    {
        if (_exporters.Count == 0)
        {
            GraphicsHelper.WriteError("Nessun esportatore disponibile.");
            return;
        }

        var weapons = _arsenal.GetAllWeapons();
        if (weapons.Count == 0)
        {
            GraphicsHelper.WriteError("Nessuna arma da esportare.");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle("ESPORTATORE");

        for (int i = 0; i < _exporters.Count; i++)
            Console.WriteLine($"   {i + 1}) {_exporters[i].FormatName}");

        Console.WriteLine("   0) Annulla");
        Console.WriteLine();
        Console.Write("   Scegli formato: ");

        if (!int.TryParse(Console.ReadLine()?.Trim(), out var choice) || choice < 0 || choice > _exporters.Count)
        {
            GraphicsHelper.WriteError("Scelta non valida.");
            return;
        }

        if (choice == 0)
        {
            Console.WriteLine("   Operazione annullata.");
            return;
        }

        var exporter = _exporters[choice - 1];
        var fileName = $"arsenale{exporter.FileExtension}";
        var fullPath = Path.GetFullPath(fileName);

        try
        {
            exporter.Export(weapons, fullPath);
            GraphicsHelper.WriteSuccess($"Arsenale esportato in: {fullPath} ({exporter.FormatName})");
        }
        catch (ExportException)
        {
            throw;
        }
    }
}
