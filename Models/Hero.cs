using DungeonQuest.Interfaces;

namespace DungeonQuest.Models;

public class Hero : ICombatant
{
    private int _hp;
    private int _xp;
    private int _gold;

    public string Name { get; }
    public Weapon? EquippedWeapon { get; set; }
    public int Level { get; private set; }
    public int BaseAttack { get; private set; }
    public int MaxHp { get; private set; }

    public int Gold
    {
        get => _gold;
        private set => _gold = Math.Max(0, value);
    }

    public int Hp
    {
        get => _hp;
        private set => _hp = Math.Max(0, value);
    }

    public int Xp
    {
        get => _xp;
        private set
        {
            _xp = value;
            while (_xp >= Level * 100)
                LevelUp();
        }
    }

    public int AttackPower => BaseAttack + (EquippedWeapon?.Damage ?? 0);

    public bool IsAlive => Hp > 0;

    public Hero(string name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Eroe" : name.Trim();
        Level = 1;
        BaseAttack = 5;
        MaxHp = 30;
        _hp = 30;
        _xp = 0;
        Gold = 0;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
    }

    public void AddReward(int gold, int xp)
    {
        Gold += gold;
        Xp += xp;
    }

    private void LevelUp()
    {
        _xp -= Level * 100;
        Level++;
        MaxHp += 10;
        _hp = MaxHp;
        BaseAttack += 2;
        Console.WriteLine($"\n{Name} è salito al livello {Level}! Vita ripristinata, attacco base ora {BaseAttack}.");
    }

    public string GetStatus()
    {
        string arma = EquippedWeapon is null ? "a mani nude" : EquippedWeapon.Name;
        return $"{Name} — Liv.{Level} | HP: {Hp}/{MaxHp} | Att: {AttackPower} | XP: {_xp}/{Level * 100} | Oro: {Gold} | Arma: {arma}";
    }
}
