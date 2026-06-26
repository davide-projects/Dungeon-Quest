using DungeonQuest.db;
using DungeonQuest.Exceptions;
using DungeonQuest.Models;
using DungeonQuest.Services;

namespace DungeonQuest.UI;

public class MenuManager
{
    private readonly DungeonContext _db;
    private readonly Hero _hero;
    private readonly ArsenalManager _arsenale;
    private readonly CombatManager _combat;
    private static readonly Random _random = new();

    public MenuManager(DungeonContext db, Hero hero)
    {
        _db = db;
        _hero = hero;
        _arsenale = new ArsenalManager(db, hero);
        _combat = new CombatManager(_arsenale);
    }

    public void Run()
    {
        int scelta;
        do
        {
            try { Console.Clear(); } catch { /* non-interactive */ }
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

            if (!int.TryParse(Console.ReadLine()?.Trim(), out scelta))
            {
                GraphicsHelper.WriteError("Input non valido. Inserisci un numero.");
                Pausa();
                scelta = -1;
                continue;
            }

            try
            {
                GestisciScelta(scelta);
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
            finally
            {
                // Pulizia eventuale risorse o stato dopo ogni operazione
            }

            if (scelta != 0)
                Pausa();

        } while (scelta != 0);

        _db.SaveChanges();
        Console.WriteLine("Ritorno al menu principale...");
    }

    private void GestisciScelta(int scelta)
    {
        switch (scelta)
        {
            case 1:
                AggiungiArma();
                break;
            case 2:
                MostraTutte();
                break;
            case 3:
                MostraPerTipo();
                break;
            case 4:
                CercaEdEquipaggia();
                break;
            case 5:
                ModificaArma();
                break;
            case 6:
                Combatti();
                break;
            case 7:
                SalvaCsv();
                break;
            case 0:
                break;
            default:
                Console.WriteLine("Scelta non valida.");
                break;
        }
    }

    private void AggiungiArma()
    {
        Console.Write("Nome arma: ");
        var nome = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(nome))
        {
            GraphicsHelper.WriteError("Il nome non può essere vuoto.");
            return;
        }

        if (!nome.All(char.IsLetter))
        {
            GraphicsHelper.WriteError("Il nome può contenere solo caratteri alfabetici.");
            return;
        }

        nome = char.ToUpper(nome[0]) + nome.Substring(1);

        Console.WriteLine();
        GraphicsHelper.WriteTitle("TIPO ARMA");
        Console.WriteLine("   1) Spada   2) Arco   3) Ascia   4) Bastone   5) Pugnale");
        Console.Write("   Scegli tipo: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var tipoScelta) || tipoScelta < 1 || tipoScelta > 5)
        {
            GraphicsHelper.WriteError("Tipo non valido.");
            return;
        }

        var tipo = (WeaponType)tipoScelta;

        Console.Write("   Danno (numero > 0): ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var danno) || danno <= 0)
        {
            GraphicsHelper.WriteError("Danno non valido. Deve essere un numero maggiore di zero.");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle("RARITÀ");
        Console.WriteLine("   1) Comune   2) Non Comune   3) Raro   4) Epico   5) Leggendario");
        Console.Write("   Scegli rarità: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var rarScelta) || rarScelta < 1 || rarScelta > 5)
        {
            GraphicsHelper.WriteError("Rarità non valida.");
            return;
        }

        var rarita = (WeaponRarity)(rarScelta - 1);

        _arsenale.AddWeapon(nome, tipo, danno, rarita);
        GraphicsHelper.WriteSuccess($"Arma '{nome}' ({rarita}) aggiunta all'arsenale!");
    }

    private void MostraTutte()
    {
        var armi = _arsenale.GetAllWeapons();
        var pozioni = _arsenale.Potions;
        bool hasWeapons = armi.Count > 0;
        bool hasPotions = pozioni.Count > 0;

        if (!hasWeapons && !hasPotions)
        {
            GraphicsHelper.WriteError("Nessun oggetto nell'inventario.");
            return;
        }

        Console.WriteLine();

        if (hasPotions)
        {
            GraphicsHelper.WriteItemList($"POZIONI ({pozioni.Count})", pozioni.Select(p => p.ToString()));
            Console.WriteLine();
        }

        if (hasWeapons)
            GraphicsHelper.WriteItemList($"ARMI ({armi.Count})", armi.Select(a => a.ToString()));
    }

    private void MostraPerTipo()
    {
        Console.WriteLine();
        GraphicsHelper.WriteTitle("TIPO ARMA");
        Console.WriteLine("   1) Spada   2) Arco   3) Ascia   4) Bastone   5) Pugnale");
        Console.Write("   Inserisci tipo: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var tipoScelta) || tipoScelta < 1 || tipoScelta > 5)
        {
            GraphicsHelper.WriteError("Tipo non valido.");
            return;
        }

        var tipo = (WeaponType)tipoScelta;
        var armi = _arsenale.GetWeaponsByType(tipo);

        if (armi.Count == 0)
        {
            GraphicsHelper.WriteError($"Nessuna arma di tipo {tipo} nell'arsenale.");
            return;
        }

        GraphicsHelper.WriteItemList(tipo.ToString(), armi.Select(a => a.ToString()));
    }

    private void CercaEdEquipaggia()
    {
        Console.Write("   Nome o parte del nome da cercare: ");
        var nome = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(nome))
        {
            GraphicsHelper.WriteError("Nome non valido.");
            return;
        }

        var risultati = _arsenale.FindAllByName(nome);

        if (risultati.Count == 0)
        {
            GraphicsHelper.WriteError($"Nessuna arma trovata con nome '{nome}'.");
            return;
        }

        if (risultati.Count == 1 && _arsenale.TryFindByName(nome, out var arma))
        {
            _hero.EquippedWeapon = arma;
            GraphicsHelper.WriteSuccess($"Equipaggiata: {arma}");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle($"RISULTATI TROVATI ({risultati.Count})");
        for (int i = 0; i < risultati.Count; i++)
            Console.WriteLine($"   {i + 1}) {risultati[i]}");

        Console.WriteLine();
        Console.Write("   Scegli un'arma da equipaggiare (0 per annullare): ");

        if (!int.TryParse(Console.ReadLine()?.Trim(), out var scelta) || scelta < 0 || scelta > risultati.Count)
        {
            GraphicsHelper.WriteError("Scelta non valida.");
            return;
        }

        if (scelta == 0)
        {
            Console.WriteLine("   Operazione annullata.");
            return;
        }

        _hero.EquippedWeapon = risultati[scelta - 1];
        GraphicsHelper.WriteSuccess($"Equipaggiata: {risultati[scelta - 1]}");
    }

    private void ModificaArma()
    {
        Console.Write("   Nome o parte del nome da cercare: ");
        var nome = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(nome))
        {
            GraphicsHelper.WriteError("Nome non valido.");
            return;
        }

        var risultati = _arsenale.FindAllByName(nome);

        if (risultati.Count == 0)
        {
            GraphicsHelper.WriteError($"Nessuna arma trovata con nome '{nome}'.");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle($"RISULTATI TROVATI ({risultati.Count})");
        for (int i = 0; i < risultati.Count; i++)
            Console.WriteLine($"   {i + 1}) {risultati[i]}");

        Console.WriteLine();
        Console.Write("   Scegli un'arma da modificare (0 per annullare): ");

        if (!int.TryParse(Console.ReadLine()?.Trim(), out var scelta) || scelta < 0 || scelta > risultati.Count)
        {
            GraphicsHelper.WriteError("Scelta non valida.");
            return;
        }

        if (scelta == 0)
        {
            Console.WriteLine("   Operazione annullata.");
            return;
        }

        var arma = risultati[scelta - 1];

        Console.WriteLine();
        GraphicsHelper.WriteTitle($"MODIFICA: {arma.Name}");

        Console.Write($"   Nome [{arma.Name}]: ");
        var nuovoNome = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(nuovoNome))
            nuovoNome = arma.Name;
        else if (!nuovoNome.All(char.IsLetter))
        {
            GraphicsHelper.WriteError("Il nome può contenere solo caratteri alfabetici.");
            return;
        }
        else
            nuovoNome = char.ToUpper(nuovoNome[0]) + nuovoNome.Substring(1);

        Console.WriteLine();
        GraphicsHelper.WriteTitle("NUOVO TIPO");
        Console.WriteLine("   1) Spada   2) Arco   3) Ascia   4) Bastone   5) Pugnale");
        Console.Write($"   Scegli tipo [{arma.Type}]: ");
        var tipoInput = Console.ReadLine()?.Trim();
        WeaponType nuovoTipo;
        if (string.IsNullOrWhiteSpace(tipoInput))
            nuovoTipo = arma.Type;
        else if (!int.TryParse(tipoInput, out var tipoScelta) || tipoScelta < 1 || tipoScelta > 5)
        {
            GraphicsHelper.WriteError("Tipo non valido.");
            return;
        }
        else
            nuovoTipo = (WeaponType)tipoScelta;

        Console.Write($"   Danno [{arma.Damage}]: ");
        var dannoInput = Console.ReadLine()?.Trim();
        int nuovoDanno;
        if (string.IsNullOrWhiteSpace(dannoInput))
            nuovoDanno = arma.Damage;
        else if (!int.TryParse(dannoInput, out nuovoDanno) || nuovoDanno <= 0)
        {
            GraphicsHelper.WriteError("Danno non valido. Deve essere un numero maggiore di zero.");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteTitle("NUOVA RARITÀ");
        Console.WriteLine("   1) Comune   2) Non Comune   3) Raro   4) Epico   5) Leggendario");
        Console.Write($"   Scegli rarità [{arma.Rarity}]: ");
        var rarInput = Console.ReadLine()?.Trim();
        WeaponRarity nuovaRarita;
        if (string.IsNullOrWhiteSpace(rarInput))
            nuovaRarita = arma.Rarity;
        else if (!int.TryParse(rarInput, out var rarScelta) || rarScelta < 1 || rarScelta > 5)
        {
            GraphicsHelper.WriteError("Rarità non valida.");
            return;
        }
        else
            nuovaRarita = (WeaponRarity)(rarScelta - 1);

        _arsenale.UpdateWeapon(arma, nuovoNome, nuovoTipo, nuovoDanno, nuovaRarita);
        GraphicsHelper.WriteSuccess($"Arma '{nuovoNome}' aggiornata!");
    }

    private void Combatti()
    {
        if (!_hero.IsAlive)
        {
            GraphicsHelper.WriteError("L'eroe è a terra! Non può combattere.");
            return;
        }

        var nemico = GeneraNemicoCasuale();
        GraphicsHelper.WriteTitle("INCONTRO!");
        Console.WriteLine(GraphicsHelper.GetEnemyArt(nemico.Name));
        Console.WriteLine();
        GraphicsHelper.WriteLineColor("  " + GraphicsHelper.GetEnemyEncounterText(nemico.Name), ConsoleColor.Magenta);
        Pausa();

        var esito = _combat.Fight(_hero, nemico);

        switch (esito)
        {
            case CombatResult.Victory:
                GraphicsHelper.WriteVictory(nemico.Name, nemico.GoldReward, nemico.XpReward);
                _hero.AddReward(nemico.GoldReward, nemico.XpReward);
                _db.SaveChanges();

                if (TentaPozione(nemico))
                    _arsenale.AddPotion(new Potion("Pozione curativa"));
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

    private void SalvaCsv()
    {
        _arsenale.SaveToCsv("arsenale.csv");
        var fullPath = Path.GetFullPath("arsenale.csv");
        GraphicsHelper.WriteSuccess($"Arsenale salvato in: {fullPath}");
    }

    private static bool TentaPozione(Enemy nemico)
    {
        double chance = nemico is Dragon ? 0.70 : 0.15;
        if (_random.NextDouble() < chance)
        {
            GraphicsHelper.WriteSuccess("Hai ottenuto una Pozione curativa!");
            return true;
        }
        return false;
    }

    private static Enemy GeneraNemicoCasuale()
    {
        return _random.Next(3) switch
        {
            0 => new Goblin(),
            1 => new Skeleton(),
            _ => new Dragon()
        };
    }

    private static void Pausa()
    {
        Console.Write("\nPremi INVIO per continuare...");
        Console.ReadLine();
    }
}
