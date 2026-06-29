namespace DungeonQuest.Models;

public class Dragon : Enemy
{
    public Dragon()
        : base("Drago", 40, 10, 30, 60, new FireBreathAttackBehavior()) { }

    public override string AsciiArt => @"
              /\___/\
             /       \
            | '-' '-' |
            \    w    /
             \  ===  /
              `-----`";

    public override string EncounterText => "Un imponente Drago atterra davanti a te!";

    public override double PotionDropChance => 0.70;
}
