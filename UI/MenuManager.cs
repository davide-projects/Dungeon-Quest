using DungeonQuest.db;
using DungeonQuest.Exceptions;
using DungeonQuest.Models;
using DungeonQuest.Services;

namespace DungeonQuest.UI;

public class MenuManager
{
    private readonly DungeonContext _db;
    private readonly Hero _hero;
    private readonly ArsenalManager _arsenal;
    private readonly CombatManager _combat;
    private static readonly Random _random = new();

    private const double DragonPotionDropChance = 0.70;
    private const double NormalPotionDropChance = 0.15;
    private const int DragonMinLevel = 2;
    private const int MaxDragonChance = 45;
    private const int DragonChancePerLevel = 5;

    public MenuManager(DungeonContext db, Hero hero)
    {
        _db = db;
        _hero = hero;
        _arsenal = new ArsenalManager(db, hero);
        _combat = new CombatManager(_arsenal);
    }

    public void Run()
    {
        int choice;
        do
        {
            try { Console.Clear(); } catch (IOException) { }
            GraphicsHelper.WriteMenuTitle();
            GraphicsHelper.WriteBox(new[]
            {
                "",
                "  " + _hero.GetStatus(),
                ""
            });
            Console.WriteLine();
            Console.WriteLine("   1) Aggiungi un'arma all'arsenale");
            Console.WriteLine("   2) Mostra inventario");
            Console.WriteLine("   3) Mostra l'arsenale per tipo");
            Console.WriteLine("   4) Cerca un'arma ed equipaggiala");
            Console.WriteLine("   5) Modifica un'arma");
            Console.WriteLine("   6) Combatti contro un nemico");
            Console.WriteLine("   7) Salva l'arsenale su file (CSV)");
            Console.WriteLine("   0) Torna al menu principale");
            Console.WriteLine();
            GraphicsHelper.WriteSeparator('-');
            Console.Write("   Scelta: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out choice))
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
                AddWeapon();
                break;
            case 2:
                ShowAll();
                break;
            case 3:
                ShowByType();
                break;
            case 4:
                SearchAndEquip();
                break;
            case 5:
                ModifyWeapon();
                break;
            case 6:
                Fight();
                break;
            case 7:
                SaveCsv();
                break;
            case 0:
                break;
            default:
                Console.WriteLine("Scelta non valida.");
                break;
        }
    }

    private void AddWeapon()
    {
        Console.Write("Nome arma: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            GraphicsHelper.WriteError("Il nome non può essere vuoto.");
            return;
        }

        if (!name.All(char.IsLetter))
        {
            GraphicsHelper.WriteError("Il nome può contenere solo caratteri alfabetici.");
            return;
        }

        name = GraphicsHelper.Capitalize(name);

        Console.WriteLine();
        GraphicsHelper.WriteTitle("TIPO ARMA");
        Console.WriteLine(GraphicsHelper.WeaponTypeMenu);
        Console.Write("   Scegli tipo: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var typeChoice) || typeChoice < 1 || typeChoice > 5)
        {
            GraphicsHelper.WriteError("Tipo non valido.");
            return;
        }

        var type = (WeaponType)typeChoice;

        Console.Write("   Danno (numero > 0): ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var damage) || damage <= 0)
        {
            GraphicsHelper.WriteError("Danno non valido. Deve essere un numero maggiore di zero.");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle("RARITÀ");
        Console.WriteLine(GraphicsHelper.RarityMenu);
        Console.Write("   Scegli rarità: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var rarityChoice) || rarityChoice < 1 || rarityChoice > 5)
        {
            GraphicsHelper.WriteError("Rarità non valida.");
            return;
        }

        var rarity = (WeaponRarity)(rarityChoice - 1);

        _arsenal.AddWeapon(name, type, damage, rarity);
        GraphicsHelper.WriteSuccess($"Arma '{name}' ({rarity}) aggiunta all'arsenale!");
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
            GraphicsHelper.WriteItemList($"POZIONI ({potions.Count})", potions.Select(p => p.ToString()));
            Console.WriteLine();
        }

        if (hasWeapons)
            GraphicsHelper.WriteItemList($"ARMI ({weapons.Count})", weapons.Select(a => a.ToString()));
    }

    private void ShowByType()
    {
        Console.WriteLine();
        GraphicsHelper.WriteTitle("TIPO ARMA");
        Console.WriteLine(GraphicsHelper.WeaponTypeMenu);
        Console.Write("   Inserisci tipo: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var typeChoice) || typeChoice < 1 || typeChoice > 5)
        {
            GraphicsHelper.WriteError("Tipo non valido.");
            return;
        }

        var type = (WeaponType)typeChoice;
        var weapons = _arsenal.GetWeaponsByType(type);

        if (weapons.Count == 0)
        {
            GraphicsHelper.WriteError($"Nessuna arma di tipo {type} nell'arsenale.");
            return;
        }

        GraphicsHelper.WriteItemList(type.ToString(), weapons.Select(a => a.ToString()));
    }

    private void SearchAndEquip()
    {
        var weapon = SearchAndChooseWeapon("da equipaggiare");
        if (weapon is null)
            return;

        _hero.EquippedWeapon = weapon;
        GraphicsHelper.WriteSuccess($"Equipaggiata: {weapon}");
    }

    private void ModifyWeapon()
    {
        var weapon = SearchAndChooseWeapon("da modificare");
        if (weapon is null)
            return;

        Console.WriteLine();
        GraphicsHelper.WriteTitle($"MODIFICA: {weapon.Name}");

        Console.Write($"   Nome [{weapon.Name}]: ");
        var newName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(newName))
            newName = weapon.Name;
        else if (!newName.All(char.IsLetter))
        {
            GraphicsHelper.WriteError("Il nome può contenere solo caratteri alfabetici.");
            return;
        }
        else
            newName = GraphicsHelper.Capitalize(newName);

        Console.WriteLine();
        GraphicsHelper.WriteTitle("NUOVO TIPO");
        Console.WriteLine(GraphicsHelper.WeaponTypeMenu);
        Console.Write($"   Scegli tipo [{weapon.Type}]: ");
        var typeInput = Console.ReadLine()?.Trim();
        WeaponType newType;
        if (string.IsNullOrWhiteSpace(typeInput))
            newType = weapon.Type;
        else if (!int.TryParse(typeInput, out var typeChoice) || typeChoice < 1 || typeChoice > 5)
        {
            GraphicsHelper.WriteError("Tipo non valido.");
            return;
        }
        else
            newType = (WeaponType)typeChoice;

        Console.Write($"   Danno [{weapon.Damage}]: ");
        var damageInput = Console.ReadLine()?.Trim();
        int newDamage;
        if (string.IsNullOrWhiteSpace(damageInput))
            newDamage = weapon.Damage;
        else if (!int.TryParse(damageInput, out newDamage) || newDamage <= 0)
        {
            GraphicsHelper.WriteError("Danno non valido. Deve essere un numero maggiore di zero.");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle("NUOVA RARITÀ");
        Console.WriteLine(GraphicsHelper.RarityMenu);
        Console.Write($"   Scegli rarità [{weapon.Rarity}]: ");
        var rarityInput = Console.ReadLine()?.Trim();
        WeaponRarity newRarity;
        if (string.IsNullOrWhiteSpace(rarityInput))
            newRarity = weapon.Rarity;
        else if (!int.TryParse(rarityInput, out var rarityChoice) || rarityChoice < 1 || rarityChoice > 5)
        {
            GraphicsHelper.WriteError("Rarità non valida.");
            return;
        }
        else
            newRarity = (WeaponRarity)(rarityChoice - 1);

        _arsenal.UpdateWeapon(weapon, newName, newType, newDamage, newRarity);
        GraphicsHelper.WriteSuccess($"Arma '{newName}' aggiornata!");
    }

    private Weapon? SearchAndChooseWeapon(string action)
    {
        Console.Write("   Nome o parte del nome da cercare: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            GraphicsHelper.WriteError("Nome non valido.");
            return null;
        }

        var results = _arsenal.FindAllByName(name);

        if (results.Count == 0)
        {
            GraphicsHelper.WriteError($"Nessuna arma trovata con nome '{name}'.");
            return null;
        }

        if (results.Count == 1)
            return results[0];

        Console.WriteLine();
        GraphicsHelper.WriteTitle($"RISULTATI TROVATI ({results.Count})");
        for (int i = 0; i < results.Count; i++)
            Console.WriteLine($"   {i + 1}) {results[i]}");

        Console.WriteLine();
        Console.Write($"   Scegli un'arma {action} (0 per annullare): ");

        if (!int.TryParse(Console.ReadLine()?.Trim(), out var choice) || choice < 0 || choice > results.Count)
        {
            GraphicsHelper.WriteError("Scelta non valida.");
            return null;
        }

        if (choice == 0)
        {
            Console.WriteLine("   Operazione annullata.");
            return null;
        }

        return results[choice - 1];
    }

    private void Fight()
    {
        if (!_hero.IsAlive)
        {
            GraphicsHelper.WriteError("L'eroe è a terra! Non può combattere.");
            return;
        }

        var enemy = GenerateRandomEnemy(_hero.Level);
        GraphicsHelper.WriteTitle("INCONTRO!");
        Console.WriteLine(GraphicsHelper.GetEnemyArt(enemy.Name));
        Console.WriteLine();
        GraphicsHelper.WriteLineColor("  " + GraphicsHelper.GetEnemyEncounterText(enemy.Name), ConsoleColor.Magenta);
        GraphicsHelper.Pause();

        var outcome = _combat.Fight(_hero, enemy);

        switch (outcome)
        {
            case CombatResult.Victory:
                GraphicsHelper.WriteVictory(enemy.Name, enemy.GoldReward, enemy.XpReward);
                _hero.AddReward(enemy.GoldReward, enemy.XpReward);
                _db.SaveChanges();

                if (_hero.LevelUpMessage is not null)
                    GraphicsHelper.WriteCombatAction(_hero.LevelUpMessage, ConsoleColor.Green);

                if (TryDropPotion(enemy))
                    _arsenal.AddPotion(new Potion("Pozione curativa"));
                break;
            case CombatResult.Defeat:
                GraphicsHelper.WriteDefeat();
                break;
            case CombatResult.Flee:
                GraphicsHelper.WriteFlee();
                break;
        }

        Console.WriteLine();
        Console.WriteLine("  " + _hero.GetStatus());
    }

    private void SaveCsv()
    {
        _arsenal.SaveToCsv("arsenale.csv");
        var fullPath = Path.GetFullPath("arsenale.csv");
        GraphicsHelper.WriteSuccess($"Arsenale salvato in: {fullPath}");
    }

    private static bool TryDropPotion(Enemy enemy)
    {
        double chance = enemy is Dragon ? DragonPotionDropChance : NormalPotionDropChance;
        if (_random.NextDouble() < chance)
        {
            GraphicsHelper.WriteSuccess("Hai ottenuto una Pozione curativa!");
            return true;
        }
        return false;
    }

    private static Enemy GenerateRandomEnemy(int heroLevel)
    {
        if (heroLevel < DragonMinLevel)
            return _random.Next(2) switch
            {
                0 => new Goblin(),
                _ => new Skeleton()
            };

        var dragonChance = Math.Min(MaxDragonChance, (heroLevel - 1) * DragonChancePerLevel);
        if (_random.Next(100) < dragonChance)
            return new Dragon();

        return _random.Next(2) switch
        {
            0 => new Goblin(),
            _ => new Skeleton()
        };
    }
}
