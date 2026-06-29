namespace DungeonQuest.Models;

public record WeaponTemplate(string Name, WeaponType Type, int Damage, WeaponRarity Rarity, int Price);

public static class WeaponCatalog
{
    private static readonly List<WeaponTemplate> AllWeapons =
    [
        new("Spada di Gondor", WeaponType.Sword, 6, WeaponRarity.Common, 0),
        new("Ascia dei Boscaioli", WeaponType.Axe, 7, WeaponRarity.Common, 0),
        new("Pugnale di Bree", WeaponType.Dagger, 4, WeaponRarity.Common, 0),
        new("Arco dei Cacciatori", WeaponType.Bow, 5, WeaponRarity.Common, 0),

        new("Spada dei Raminghi", WeaponType.Sword, 10, WeaponRarity.Uncommon, 25),
        new("Ascia Nanica da Battaglia", WeaponType.Axe, 11, WeaponRarity.Uncommon, 25),
        new("Arco Silvano", WeaponType.Bow, 9, WeaponRarity.Uncommon, 25),
        new("Pugnale di Dol Guldur", WeaponType.Dagger, 8, WeaponRarity.Uncommon, 25),

        new("Lama di Numenor", WeaponType.Sword, 15, WeaponRarity.Rare, 60),
        new("Ascia di Ferro Freddo", WeaponType.Axe, 16, WeaponRarity.Rare, 60),
        new("Arco di Lorien", WeaponType.Bow, 14, WeaponRarity.Rare, 60),
        new("Bastone del Guardiano Grigio", WeaponType.Staff, 13, WeaponRarity.Rare, 60),

        new("Spada di Gondolin", WeaponType.Sword, 22, WeaponRarity.Epic, 140),
        new("Ascia di Durin", WeaponType.Axe, 24, WeaponRarity.Epic, 140),
        new("Arco di Thranduil", WeaponType.Bow, 21, WeaponRarity.Epic, 140),
        new("Bastone di Radagast", WeaponType.Staff, 20, WeaponRarity.Epic, 140),

        new("Anduril, Fiamma dell'Ovest", WeaponType.Sword, 32, WeaponRarity.Legendary, 300),
        new("Glamdring", WeaponType.Sword, 30, WeaponRarity.Legendary, 300),
        new("Ascia di Gimli", WeaponType.Axe, 31, WeaponRarity.Legendary, 300),
        new("Arco di Legolas", WeaponType.Bow, 29, WeaponRarity.Legendary, 300),
        new("Bastone di Gandalf il Bianco", WeaponType.Staff, 28, WeaponRarity.Legendary, 300),
        new("Pugnale di Bilbo (Pungolo)", WeaponType.Dagger, 26, WeaponRarity.Legendary, 300),
    ];

    private static readonly Random _random = new();

    public static IReadOnlyList<WeaponTemplate> All => AllWeapons;

    public static WeaponTemplate GetRandomCommon()
    {
        var commons = AllWeapons.Where(w => w.Rarity == WeaponRarity.Common).ToList();
        return commons[_random.Next(commons.Count)];
    }
}
