using DungeonQuest.Interfaces;

namespace DungeonQuest.Models;

public abstract class Enemy : ICombatant
{
    private static readonly Random SharedRandom = new();
    private int _hp;

    public string Name { get; }
    public int MaxHp { get; }
    public int Attack { get; }
    public int GoldReward { get; }
    public int XpReward { get; }
    public IAttackBehavior AttackBehavior { get; }

    public int Hp
    {
        get => _hp;
        private set => _hp = Math.Max(0, value);
    }

    public int AttackPower => Attack;
    public bool IsAlive => Hp > 0;

    public abstract string AsciiArt { get; }
    public abstract string EncounterText { get; }
    public virtual double PotionDropChance => 0.15;

    protected Enemy(string name, int hp, int attack, int goldReward, int xpReward, IAttackBehavior attackBehavior)
    {
        Name = name;
        MaxHp = hp;
        _hp = hp;
        Attack = attack;
        GoldReward = goldReward;
        XpReward = xpReward;
        AttackBehavior = attackBehavior;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
    }

    public string GetStatus()
    {
        return $"{Name} — HP: {Hp}/{MaxHp}";
    }

    public virtual bool TryDropPotion()
    {
        if (SharedRandom.NextDouble() < PotionDropChance)
            return true;
        return false;
    }
}
