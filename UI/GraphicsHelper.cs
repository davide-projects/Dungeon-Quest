namespace DungeonQuest.UI;

public static class GraphicsHelper
{
    private const int BoxWidth = 100;

    public static void WriteColor(string text, ConsoleColor color)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = prev;
    }

    public static void WriteLineColor(string text, ConsoleColor color)
    {
        WriteColor(text, color);
        Console.WriteLine();
    }

    public static void WriteSeparator(char c = '=')
    {
        Console.WriteLine(new string(c, BoxWidth));
    }

    public static void WriteTitle(string title)
    {
        WriteSeparator('=');
        var pad = (BoxWidth - title.Length) / 2;
        var left = pad > 0 ? pad : 0;
        var right = BoxWidth - title.Length - left;
        Console.WriteLine("║" + new string(' ', left) + title + new string(' ', right) + "║");
        WriteSeparator('=');
    }

    public static void WriteBox(IEnumerable<string> lines)
    {
        WriteSeparator('-');
        foreach (var line in lines)
        {
            var content = line.Length > BoxWidth - 4 ? line[..(BoxWidth - 7)] + "..." : line;
            Console.WriteLine("| " + content.PadRight(BoxWidth - 4) + " |");
        }
        WriteSeparator('-');
    }

    public static void WriteMenuTitle()
    {
        WriteSeparator('=');
        Console.WriteLine("╔" + new string('=', BoxWidth - 2) + "╗");
        var title = "§ § §  DUNGEON QUEST  § § §";
        var subtitle = ">>  Avventura testuale  <<";
        var pad1 = (BoxWidth - title.Length) / 2;
        var pad2 = (BoxWidth - subtitle.Length) / 2;
        Console.WriteLine("║" + new string(' ', BoxWidth - 2) + "║");
        Console.WriteLine("║" + new string(' ', pad1) + title + new string(' ', BoxWidth - 2 - pad1 - title.Length) + "║");
        Console.WriteLine("║" + new string(' ', pad2) + subtitle + new string(' ', BoxWidth - 2 - pad2 - subtitle.Length) + "║");
        Console.WriteLine("║" + new string(' ', BoxWidth - 2) + "║");
        Console.WriteLine("╚" + new string('=', BoxWidth - 2) + "╝");
        WriteSeparator('=');
    }

    public static void WriteHealthBar(int current, int max, int barLength = 20)
    {
        var filled = (int)((double)current / max * barLength);
        if (filled > barLength) filled = barLength;
        if (filled < 0) filled = 0;

        var prevBg = Console.BackgroundColor;
        var prevFg = Console.ForegroundColor;

        Console.Write("[");
        Console.BackgroundColor = current > max / 2 ? ConsoleColor.Green :
                                   current > max / 4 ? ConsoleColor.DarkYellow :
                                   ConsoleColor.Red;
        Console.Write(new string('█', filled));
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('▓', barLength - filled));
        Console.BackgroundColor = prevBg;
        Console.Write("]");
    }

    public static string GetEnemyArt(string enemyName)
    {
        return enemyName.ToLowerInvariant() switch
        {
            "goblin" => @"
               .-.
              /   \
             | .-. |
             | |_| |
              `---'
              _/   \_",
            "scheletro" => @"
               .-.
              /ooo\
             | .-. |
             | |_| |
              `---'
               | |
              /   \",
            
            "drago" => @"
              /\___/\
             /       \
            | '-' '-' |
            \    w    /
             \  ===  /
              `-----`",
            
            _ => @"
               .-.
              (o o)
              | O |
              `---'"
        };
    }

    public static string GetEnemyEncounterText(string enemyName)
    {
        return enemyName.ToLowerInvariant() switch
        {
            "goblin" => "Un Goblin furtivo esce dalle ombre!",
            "scheletro" => "Uno Scheletro risorge dal suolo!",
            "drago" => "Un imponente Drago atterra davanti a te!",
            _ => $"Un {enemyName} appare!"
        };
    }

    public static void WriteCombatHeader(string heroName, string enemyName, int heroHp, int heroMaxHp, int enemyHp, int enemyMaxHp)
    {
        Console.Clear();
        WriteSeparator('=');
        WriteColor(" § § §  COMBATTIMENTO  § § §", ConsoleColor.Red);
        Console.WriteLine();
        WriteSeparator('=');
        Console.WriteLine();
        WriteColor(" << ", ConsoleColor.Cyan);
        Console.Write($"{heroName}  ");
        WriteHealthBar(heroHp, heroMaxHp);
        Console.WriteLine($"  {heroHp}/{heroMaxHp}");
        Console.WriteLine();
        WriteColor(" >> ", ConsoleColor.Red);
        Console.Write($"{enemyName}  ");
        WriteHealthBar(enemyHp, enemyMaxHp);
        Console.WriteLine($"  {enemyHp}/{enemyMaxHp}");
        Console.WriteLine();
    }

    public static void WriteCombatAction(string message, ConsoleColor color)
    {
        Console.Write("  >> ");
        WriteLineColor(message, color);
    }

    public static void WriteVictory(string enemyName, int gold, int xp)
    {
        Console.WriteLine();
        WriteSeparator('=');
        WriteLineColor("** VITTORIA ** Hai sconfitto " + enemyName + "!", ConsoleColor.Green);
        Console.WriteLine("   Ricompensa: " + gold + " oro, " + xp + " XP");
        WriteSeparator('=');
    }

    public static void WriteDefeat()
    {
        Console.WriteLine();
        WriteSeparator('=');
        WriteLineColor("!! SCONFITTA !! L'eroe e' a terra!", ConsoleColor.Red);
        WriteSeparator('=');
    }

    public static void WriteFlee()
    {
        Console.WriteLine();
        WriteSeparator('=');
        WriteLineColor(">> FUGGITO << Sei fuggito dallo scontro!", ConsoleColor.Yellow);
        WriteSeparator('=');
    }

    public static void WriteItemList(string title, IEnumerable<string> items)
    {
        WriteTitle(title);
        foreach (var item in items)
            Console.WriteLine("  * " + item);
    }

    public static void WriteError(string message)
    {
        WriteColor("!! ", ConsoleColor.Yellow);
        WriteLineColor(message, ConsoleColor.Red);
    }

    public static void WriteSuccess(string message)
    {
        WriteColor(">> ", ConsoleColor.Green);
        Console.WriteLine(message);
    }

    public static void Pause()
    {
        Console.Write("\n   Premi INVIO per continuare...");
        Console.ReadLine();
    }

    public static string Capitalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;
        return char.ToUpper(input[0]) + input.Substring(1);
    }

    public const string WeaponTypeMenu = "   1) Spada   2) Arco   3) Ascia   4) Bastone   5) Pugnale";
    public const string RarityMenu = "   1) Comune   2) Non Comune   3) Raro   4) Epico   5) Leggendario";
}
