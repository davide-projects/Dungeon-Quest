namespace DungeonQuest.Models;

public class Goblin : Enemy
{
    public Goblin()
        : base("Goblin", 12, 4, 5, 15, new NormalAttackBehavior()) { }

    public override string AsciiArt => @"
               .-.
              /   \
             | .-. |
             | |_| |
              `---'
             _/   \_";

    public override string EncounterText => "Un Goblin furtivo esce dalle ombre!";
}
