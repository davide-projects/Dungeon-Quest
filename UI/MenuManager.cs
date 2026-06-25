using DungeonQuest.Exceptions;
using DungeonQuest.Models;
using DungeonQuest.Services;

namespace DungeonQuest.UI;

public class MenuManager
{
    private readonly Hero _hero;
    private readonly ArsenalManager _arsenale;
    private readonly CombatManager _combat;
    private static readonly Random _random = new();

    public MenuManager(Hero hero)
    {
        _hero = hero;
        _arsenale = new ArsenalManager(hero);
        _arsenale.LoadFromCsv("arsenale.csv");
        _combat = new CombatManager();
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
            Console.WriteLine("   2) Mostra tutto l'arsenale");
            Console.WriteLine("   3) Mostra l'arsenale per tipo");
            Console.WriteLine("   4) Cerca un'arma ed equipaggiala");
            Console.WriteLine("   5) Combatti contro un nemico");
            Console.WriteLine("   6) Salva l'arsenale su file (CSV)");
            Console.WriteLine("   0) Esci");
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

        Console.WriteLine("Arrivederci!");
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
                Combatti();
                break;
            case 6:
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

        var tipo = (WeaponType)(tipoScelta - 1);

        Console.Write("   Danno (numero > 0): ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var danno) || danno <= 0)
        {
            GraphicsHelper.WriteError("Danno non valido. Deve essere un numero maggiore di zero.");
            return;
        }

        _arsenale.AddWeapon(nome, tipo, danno);
        GraphicsHelper.WriteSuccess($"Arma '{nome}' aggiunta all'arsenale!");
    }

    private void MostraTutte()
    {
        var armi = _arsenale.GetAllWeapons();
        if (armi.Count == 0)
        {
            GraphicsHelper.WriteError("Nessuna arma nell'arsenale.");
            return;
        }

        Console.WriteLine();
        GraphicsHelper.WriteItemList("ARSENALE COMPLETO", armi.Select(a => a.ToString()));
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

        var tipo = (WeaponType)(tipoScelta - 1);
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

        if (risultati.Count == 1)
        {
            _hero.EquippedWeapon = risultati[0];
            GraphicsHelper.WriteSuccess($"Equipaggiata: {risultati[0]}");
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
