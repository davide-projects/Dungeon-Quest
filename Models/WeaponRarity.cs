using DungeonQuest.Exceptions;

namespace DungeonQuest.Models;

public class WeaponRarity
{
    public static readonly WeaponRarity Common = new(1, "Comune");
    public static readonly WeaponRarity Uncommon = new(2, "Non Comune");
    public static readonly WeaponRarity Rare = new(3, "Raro");
    public static readonly WeaponRarity Epic = new(4, "Epico");
    public static readonly WeaponRarity Legendary = new(5, "Leggendario");

    private static readonly List<WeaponRarity> AllRarities = [Common, Uncommon, Rare, Epic, Legendary];
    public static IReadOnlyList<WeaponRarity> All => AllRarities;

    public int Id { get; }
    public string DisplayName { get; }

    private WeaponRarity(int id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public static WeaponRarity FromId(int id) =>
        AllRarities.FirstOrDefault(wr => wr.Id == id)
        ?? throw new InvalidWeaponRarityException($"Rarità '{id}' non valida.");

    public override string ToString() => DisplayName;
}
