using DungeonQuest.Interfaces;

namespace DungeonQuest.Models;

public abstract class Enemy : ICombatant
{
    private static readonly Random SharedRandom = new();
    private int _hp;
    private readonly int _baseMaxHp;
    private readonly int _baseAttack;
    private readonly int _baseGoldReward;
    private readonly int _baseXpReward;

    public string Name { get; }
    public int MaxHp { get; private set; }
    public int Attack { get; private set; }
    public int GoldReward { get; private set; }
    public int XpReward { get; private set; }
    public int MinLevel { get; }
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
    public virtual double XpBoostPotionDropChance => 0.0;

    protected Enemy(string name, int hp, int attack, int goldReward, int xpReward, IAttackBehavior attackBehavior, int minLevel = 1)
    {
        Name = name;
        _baseMaxHp = hp;
        _baseAttack = attack;
        _baseGoldReward = goldReward;
        _baseXpReward = xpReward;
        MaxHp = hp;
        _hp = hp;
        Attack = attack;
        GoldReward = goldReward;
        XpReward = xpReward;
        MinLevel = minLevel;
        AttackBehavior = attackBehavior;
    }

    public void ScaleToLevel(int heroLevel)
    {
        int levelsAboveMin = heroLevel - MinLevel;
        if (levelsAboveMin <= 0) return;

        double factor = 1.0 + levelsAboveMin * 0.15;
        MaxHp = (int)(_baseMaxHp * factor);
        Attack = (int)(_baseAttack * factor);
        GoldReward = (int)(_baseGoldReward * (1.0 + levelsAboveMin * 0.10));
        XpReward = (int)(_baseXpReward * (1.0 + levelsAboveMin * 0.10));
        if (_hp > MaxHp) _hp = MaxHp;
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

    public virtual bool TryDropXpBoostPotion()
    {
        if (SharedRandom.NextDouble() < XpBoostPotionDropChance)
            return true;
        return false;
    }

    public virtual string OnDefeated(Hero hero) => string.Empty;
}
