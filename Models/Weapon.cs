using DungeonQuest.Exceptions;
using DungeonQuest.Utilities;

namespace DungeonQuest.Models;

public class Weapon
{
    private string _name = string.Empty;
    private int _damage;

    public int Code { get; }

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

    public WeaponType Type { get; private set; }

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

    public Weapon(string name, WeaponType type, int damage)
    {
        Code = IdGenerator.NextWeaponId();
        Name = name;
        Type = type;
        Damage = damage;
    }

    internal Weapon(int code, string name, WeaponType type, int damage)
    {
        Code = code;
        _name = name;
        Type = type;
        _damage = damage;
    }

    public override string ToString()
    {
        return $"#{Code} {Name} ({Type}) — danno {Damage}.";
    }
}