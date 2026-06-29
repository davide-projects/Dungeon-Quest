using DungeonQuest.Exceptions;

namespace DungeonQuest.Models;

public class WeaponType
{
    public static readonly WeaponType Sword = new(1, "Spada");
    public static readonly WeaponType Bow = new(2, "Arco");
    public static readonly WeaponType Axe = new(3, "Ascia");
    public static readonly WeaponType Staff = new(4, "Bastone");
    public static readonly WeaponType Dagger = new(5, "Pugnale");

    private static readonly List<WeaponType> AllTypes = [Sword, Bow, Axe, Staff, Dagger];
    public static IReadOnlyList<WeaponType> All => AllTypes;

    public int Id { get; }
    public string DisplayName { get; }

    private WeaponType(int id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public static WeaponType FromId(int id) =>
        AllTypes.FirstOrDefault(wt => wt.Id == id)
        ?? throw new InvalidWeaponTypeException($"Tipo arma '{id}' non valido.");

    public override string ToString() => DisplayName;
}
