namespace DungeonQuest.Interfaces;

public class AttackResult
{
    public int Damage { get; set; }
    public string Description { get; set; } = string.Empty;
}

public interface IAttackBehavior
{
    AttackResult Execute(int baseAttack);
}
