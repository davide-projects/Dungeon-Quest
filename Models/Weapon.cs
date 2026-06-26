using DungeonQuest.Exceptions;

namespace DungeonQuest.Models;

public class Weapon
{
    private string _name = string.Empty;
    private int _damage;
    private WeaponType _type;
    private WeaponRarity _rarity;

    public int Code { get; }

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

    public WeaponType Type
    {
        get => _type;
        private set
        {
            if (!Enum.IsDefined(typeof(WeaponType), value))
                throw new InvalidWeaponTypeException($"Tipo '{value}' non valido. Valori validi: Spada(1), Arco(2), Ascia(3), Bastone(4), Pugnale(5).");

            _type = value;
        }
    }

    public WeaponRarity Rarity
    {
        get => _rarity;
        private set
        {
            if (!Enum.IsDefined(typeof(WeaponRarity), value))
                throw new InvalidWeaponRarityException($"Rarità '{value}' non valida. Valori validi: Comune, NonComune, Raro, Epico, Leggendario.");

            _rarity = value;
        }
    }

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

    public Weapon(string name, WeaponType type, int damage, WeaponRarity rarity = WeaponRarity.Comune)
    {
        Name = name;
        Type = type;
        Damage = damage;
        Rarity = rarity;
    }

    internal Weapon(int code, string name, WeaponType type, int damage, WeaponRarity rarity = WeaponRarity.Comune)
    {
        Code = code;
        _name = name;
        Type = type;
        _damage = damage;
        Rarity = rarity;
    }

    public void Update(string name, WeaponType type, int damage, WeaponRarity rarity)
    {
        Name = name;
        Type = type;
        Damage = damage;
        Rarity = rarity;
    }

    public override string ToString()
    {
        return $"#{Code} {Name} ({Type}) — [{Rarity}] danno {Damage}.";
    }
}