using DungeonQuest.Exceptions;

namespace DungeonQuest.Models;

public class Weapon
{
    private string _name = string.Empty;
    private int _damage;

    public int Id { get; }

    public int? HeroId { get; set; }
    public Hero? Hero { get; set; }

    public int? EquippedByHeroId { get; set; }

    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidWeaponNameException("Il nome dell'arma non può essere vuoto.");

            _name = value.Trim();
        }
    }

    public WeaponType Type { get; private set; } = null!;

    public WeaponRarity Rarity { get; private set; } = null!;

    public int Damage
    {
        get => _damage;
        private set
        {
            if (value <= 0)
                throw new InvalidWeaponDamageException("Il danno dell'arma deve essere maggiore di zero.");

            _damage = value;
        }
    }

    public Weapon(string name, WeaponType type, int damage, WeaponRarity rarity)
    {
        Name = name;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Damage = damage;
        Rarity = rarity ?? throw new ArgumentNullException(nameof(rarity));
    }

    internal Weapon(int id, string name, WeaponType type, int damage, WeaponRarity rarity) : this(name, type, damage, rarity)
    {
        Id = id;
    }

    public void Update(string name, WeaponType type, int damage, WeaponRarity rarity)
    {
        Name = name;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Damage = damage;
        Rarity = rarity ?? throw new ArgumentNullException(nameof(rarity));
    }

    public override string ToString()
    {
        return $"#{Id} {Name} ({Type}) \u2014 [{Rarity}] danno {Damage}.";
    }
}
