using DungeonQuest.Exceptions;

namespace DungeonQuest.Models;

public class WeaponRarity
{
    public static readonly WeaponRarity Common = new(1, "Comune", 0);
    public static readonly WeaponRarity Uncommon = new(2, "Non Comune", 1);
    public static readonly WeaponRarity Rare = new(3, "Raro", 2);
    public static readonly WeaponRarity Epic = new(4, "Epico", 3);
    public static readonly WeaponRarity Legendary = new(5, "Leggendario", 4);

    private static readonly List<WeaponRarity> AllRarities = [Common, Uncommon, Rare, Epic, Legendary];
    public static IReadOnlyList<WeaponRarity> All => AllRarities;

    public int Id { get; }
    public string DisplayName { get; }
    public int BonusAttack { get; }

    private WeaponRarity(int id, string displayName, int bonusAttack)
    {
        Id = id;
        DisplayName = displayName;
        BonusAttack = bonusAttack;
    }

    public static WeaponRarity FromId(int id) =>
        AllRarities.FirstOrDefault(wr => wr.Id == id)
        ?? throw new InvalidWeaponRarityException($"Rarità '{id}' non valida.");

    public override string ToString() => DisplayName;
}
