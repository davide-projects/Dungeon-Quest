using DungeonQuest.Models;

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
        Console.WriteLine("\u2551" + new string(' ', left) + title + new string(' ', right) + "\u2551");
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
        Console.WriteLine("\u2554" + new string('=', BoxWidth - 2) + "\u2557");
        var title = "\u00a7 \u00a7 \u00a7  DUNGEON QUEST  \u00a7 \u00a7 \u00a7";
        var subtitle = ">>  Avventura testuale  <<";
        var pad1 = (BoxWidth - title.Length) / 2;
        var pad2 = (BoxWidth - subtitle.Length) / 2;
        Console.WriteLine("\u2551" + new string(' ', BoxWidth - 2) + "\u2551");
        Console.WriteLine("\u2551" + new string(' ', pad1) + title + new string(' ', BoxWidth - 2 - pad1 - title.Length) + "\u2551");
        Console.WriteLine("\u2551" + new string(' ', pad2) + subtitle + new string(' ', BoxWidth - 2 - pad2 - subtitle.Length) + "\u2551");
        Console.WriteLine("\u2551" + new string(' ', BoxWidth - 2) + "\u2551");
        Console.WriteLine("\u255a" + new string('=', BoxWidth - 2) + "\u255d");
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
        Console.Write(new string('\u2588', filled));
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('\u2593', barLength - filled));
        Console.BackgroundColor = prevBg;
        Console.Write("]");
    }

    public static void WriteCombatHeader(string heroName, string enemyName, int heroHp, int heroMaxHp, int enemyHp, int enemyMaxHp)
    {
        Console.Clear();
        WriteSeparator('=');
        WriteColor(" \u00a7 \u00a7 \u00a7  COMBATTIMENTO  \u00a7 \u00a7 \u00a7", ConsoleColor.Red);
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

    public static void WriteBossVictory(string enemyName, int gold, int xp)
    {
        Console.Clear();
        WriteSeparator('=');
        var victoryArt = new[]
        {
            @"  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
            @"  ~                                                             ~",
            @"  ~     *******  **    **  ********  *******   *******          ~",
            @"  ~    /**////**/**   /** **//////  /**////** /**////**         ~",
            @"  ~    /**   /**//** /** /**        /**   /** /**   /**         ~",
            @"  ~    /*******  //****  /********* /*******  /*******          ~",
            @"  ~    /**////    //**   ////////** /**///**  /**///**          ~",
            @"  ~    /**         /**          /** /**  /**  /**  /**          ~",
            @"  ~    /**         /**    ********  /**   /** /**   /**         ~",
            @"  ~    //          //    ////////   //    //  //    //          ~",
            @"  ~                                                             ~",
            @"  ~     ********  **      ** ******** ******** ********         ~",
            @"  ~    /**/////  /**     /**//////// /**///// /**/////          ~",
            @"  ~    /**       /**     /**       /** /**      /**             ~",
            @"  ~    /*******  /**     /**      /** /******* /*******         ~",
            @"  ~    /**////   /**     /**     /**  /**////  /**////          ~",
            @"  ~    /**       /**     /**    /**   /**      /**             ~",
            @"  ~    /**       //*******     /********/**      /**             ~",
            @"  ~    //         ///////      //////// //       //              ~",
            @"  ~                                                             ~",
            @"  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
        };
        Console.ForegroundColor = ConsoleColor.Yellow;
        foreach (var line in victoryArt)
        {
            var pad = (BoxWidth - line.Length) / 2;
            Console.WriteLine(new string(' ', Math.Max(0, pad)) + line);
        }
        Console.ResetColor();
        Console.WriteLine();
        WriteSeparator('=');
        WriteLineColor("  HAI SCONFITTO " + enemyName.ToUpperInvariant() + "!", ConsoleColor.Green);
        Console.WriteLine("  Ricompensa: " + gold + " oro, " + xp + " XP");
        WriteSeparator('=');
        Console.WriteLine();
        Console.Write("   Premi INVIO per uscire...");
        Console.ReadLine();
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

    public static void WriteWeapon(Weapon weapon, string prefix = "")
    {
        WriteColor($"{prefix}#{weapon.Id} ", ConsoleColor.DarkGray);
        WriteColor($"{weapon.Name} ", ConsoleColor.White);
        WriteColor($"({weapon.Type}) ", ConsoleColor.DarkCyan);
        Console.Write("\u2014 ");
        WriteColor($"[{weapon.Rarity}]", GetRarityColor(weapon.Rarity));
        WriteColor(" danno ", ConsoleColor.Gray);
        WriteColor($"{weapon.Damage}.", ConsoleColor.Cyan);
        Console.WriteLine();
    }

    public static void WriteWeapon(string name, WeaponType type, int damage, WeaponRarity rarity, string prefix = "")
    {
        WriteColor(prefix, ConsoleColor.White);
        WriteColor($"{name} ", ConsoleColor.White);
        WriteColor($"({type}) ", ConsoleColor.DarkCyan);
        Console.Write("\u2014 ");
        WriteColor($"[{rarity}]", GetRarityColor(rarity));
        WriteColor(" danno ", ConsoleColor.Gray);
        WriteColor($"{damage}.", ConsoleColor.Cyan);
        Console.WriteLine();
    }

    public static void WritePotion(Potion potion, string prefix = "")
    {
        WriteColor($"{prefix}{potion.Name} ", ConsoleColor.Green);
        string desc = potion.Kind == PotionKind.XpBoost
            ? "(moltiplica XP del prossimo combattimento x2)"
            : "(cura il 50% della salute massima)";
        WriteColor(desc, potion.Kind == PotionKind.XpBoost ? ConsoleColor.Cyan : ConsoleColor.DarkGreen);
        Console.WriteLine();
    }

    public static void WriteItemList(string title, IEnumerable<Weapon> weapons)
    {
        WriteTitle(title);
        foreach (var weapon in weapons)
            WriteWeapon(weapon, "  * ");
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

    public static string GetWeaponTypeMenu()
    {
        int i = 1;
        return string.Join("   ", WeaponType.All.Select(wt => $"{i++}) {wt.DisplayName}"));
    }

    public static string GetRarityMenu()
    {
        int i = 1;
        return string.Join("   ", WeaponRarity.All.Select(r => $"{i++}) {r.DisplayName}"));
    }

    public static int WeaponTypeCount => WeaponType.All.Count;
    public static int WeaponRarityCount => WeaponRarity.All.Count;

    public static int ParseWeaponTypeChoice(string input)
    {
        if (!int.TryParse(input, out var choice))
            return -1;
        if (choice < 1 || choice > WeaponType.All.Count)
            return -1;
        return choice;
    }

    public static int ParseRarityChoice(string input)
    {
        if (!int.TryParse(input, out var choice))
            return -1;
        if (choice < 1 || choice > WeaponRarity.All.Count)
            return -1;
        return choice;
    }

    public static WeaponType? GetWeaponTypeFromChoice(int choice)
    {
        if (choice < 1 || choice > WeaponType.All.Count)
            return null;
        return WeaponType.All[choice - 1];
    }

    public static WeaponRarity? GetRarityFromChoice(int choice)
    {
        if (choice < 1 || choice > WeaponRarity.All.Count)
            return null;
        return WeaponRarity.All[choice - 1];
    }

    public static ConsoleColor GetRarityColor(WeaponRarity rarity)
    {
        return rarity.Id switch
        {
            1 => ConsoleColor.Gray,
            2 => ConsoleColor.Green,
            3 => ConsoleColor.Cyan,
            4 => ConsoleColor.Magenta,
            5 => ConsoleColor.Yellow,
            _ => ConsoleColor.Gray
        };
    }

    public static void WriteHeroStatus(Hero hero)
    {
        WriteColor($"{hero.Name} — ", ConsoleColor.White);
        WriteColor($"Liv.{hero.Level}", ConsoleColor.Cyan);
        Console.Write(" | ");
        WriteColor("HP:", ConsoleColor.White);
        WriteColor($" {hero.Hp}/{hero.MaxHp} ", hero.Hp > hero.MaxHp / 2
            ? ConsoleColor.Green
            : hero.Hp > hero.MaxHp / 4
                ? ConsoleColor.DarkYellow
                : ConsoleColor.Red);
        Console.Write("| ");
        WriteColor("Att:", ConsoleColor.White);
        WriteColor($" {hero.BaseAttack}", ConsoleColor.DarkCyan);
        Console.Write("+");
        WriteColor($"{hero.WeaponDamage}", ConsoleColor.Cyan);
        if (hero.RarityBonus > 0)
        {
            Console.Write("+");
            WriteColor($"{hero.RarityBonus}", GetRarityColor(hero.EquippedWeapon!.Rarity));
        }
        WriteColor($"={hero.AttackPower}", ConsoleColor.White);
        Console.Write(" | ");
        WriteColor("XP:", ConsoleColor.White);
        WriteColor($" {hero.Xp}/{hero.Level * 100}", ConsoleColor.DarkYellow);
        Console.Write(" | ");
        WriteColor("Oro:", ConsoleColor.White);
        WriteColor($" {hero.Gold}", ConsoleColor.Yellow);
        if (hero.EquippedWeapon is not null)
        {
            Console.Write(" | ");
            WriteColor("Arma:", ConsoleColor.White);
            Console.Write($" {hero.EquippedWeapon.Name} ");
            WriteColor($"({hero.EquippedWeapon.Rarity})", GetRarityColor(hero.EquippedWeapon.Rarity));
        }
        Console.WriteLine();
    }
}
