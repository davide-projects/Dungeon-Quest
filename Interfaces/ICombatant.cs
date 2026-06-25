namespace DungeonQuest.Interfaces;

public interface ICombatant
{
    string Name { get; }
    int Hp { get; }
    int MaxHp { get; }
    int AttackPower { get; }
    bool IsAlive { get; }
    void TakeDamage(int damage);
}
