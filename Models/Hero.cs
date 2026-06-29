using System.ComponentModel.DataAnnotations.Schema;
using DungeonQuest.Interfaces;

namespace DungeonQuest.Models;

public class Hero : ICombatant
{
    private const int XpPerLevel = 100;
    private const int MaxHeroLevel = 10;
    private const int StartingLevel = 1;
    private const int StartingBaseAttack = 5;
    private const int StartingMaxHp = 30;
    private const int AttackPerLevel = 2;
    private const int HpPerLevel = 10;
    private const int BaseMissChance = 20;
    private const int MissReductionPerLevel = 2;
    private const int MinMissChance = 2;

    private int _hp;
    private int _xp;
    private int _gold;
    private int _xpBoostRemaining;

    public int Id { get; set; }
    public string Name { get; private set; }
    public Weapon? EquippedWeapon { get; set; }
    public List<Weapon> Weapons { get; set; } = [];
    public int Level { get; private set; }
    public int MissRate => Math.Max(MinMissChance, BaseMissChance - (Level - 1) * MissReductionPerLevel);
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

    public int WeaponDamage => EquippedWeapon?.Damage ?? 0;
    public int RarityBonus => EquippedWeapon?.Rarity.BonusAttack ?? 0;
    public int AttackPower => BaseAttack + WeaponDamage + RarityBonus;

    public bool IsAlive => Hp > 0;

    [NotMapped]
    public string? LevelUpMessage { get; set; }

    public int XpBoostRemaining
    {
        get => _xpBoostRemaining;
        set => _xpBoostRemaining = Math.Max(0, value);
    }

    private Hero() { Name = null!; }

    public Hero(string name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Eroe" : name.Trim();
        Level = StartingLevel;
        BaseAttack = StartingBaseAttack;
        MaxHp = StartingMaxHp;
        _hp = StartingMaxHp;
        _xp = 0;
        _xpBoostRemaining = 0;
        Gold = 0;
    }

    public void Revive()
    {
        if (_hp <= 0)
            _hp = MaxHp;
    }

    public void Reset()
    {
        _hp = StartingMaxHp;
        _xp = 0;
        _xpBoostRemaining = 0;
        _gold = 0;
        Level = StartingLevel;
        BaseAttack = StartingBaseAttack;
        MaxHp = StartingMaxHp;
        EquippedWeapon = null;
    }

    public string GetShortStatus()
    {
        string state = Hp <= 0 ? " (Sconfitto!)" : "";
        string rarity = EquippedWeapon is null ? "" : $" (rarità +{RarityBonus})";
        return $"{Name} — Liv.{Level} | HP: {Hp}/{MaxHp} | Att: {AttackPower}{rarity} | Oro: {Gold}{state}";
    }

    public void Heal(int amount)
    {
        _hp = Math.Min(MaxHp, _hp + amount);
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
        if (Level >= MaxHeroLevel)
        {
            _xp = Level * XpPerLevel;
            LevelUpMessage = null;
            return;
        }

        _xp -= Level * XpPerLevel;
        Level++;
        MaxHp += HpPerLevel;
        _hp = MaxHp;
        BaseAttack += AttackPerLevel;
        string attackDetail = RarityBonus > 0
            ? $"{BaseAttack}+{WeaponDamage}+{RarityBonus}"
            : $"{BaseAttack}+{WeaponDamage}";
        LevelUpMessage = $"{Name} è salito al livello {Level}! HP: {_hp}/{MaxHp} | Att: {attackDetail}={AttackPower} | Tasso errore: {MissRate}%";
    }

    public string GetStatus()
    {
        string weapon = EquippedWeapon is null ? "a mani nude" : EquippedWeapon.Name;
        string attackDetail = RarityBonus > 0
            ? $"{BaseAttack}+{WeaponDamage}+{RarityBonus}"
            : $"{BaseAttack}+{WeaponDamage}";
        return $"{Name} — Liv.{Level} | HP: {Hp}/{MaxHp} | Att: {attackDetail}={AttackPower} | XP: {_xp}/{Level * 100} | Oro: {Gold} | Arma: {weapon}";
    }
}
