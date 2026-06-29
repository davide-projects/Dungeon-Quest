namespace DungeonQuest.Models;

public class Orc : Enemy
{
    public Orc()
        : base("Orchetto", 22, 7, 12, 30, new NormalAttackBehavior()) { }

    public override double PotionDropChance => 0.20;
    public override double XpBoostPotionDropChance => 0.10;

    public override string AsciiArt => @"
               .   .
              / \_/ \
             | .-. .-|
             | |_| |_|
              \_____/
             _/     \_
             /  ___   \
            |  / _ \  |
            | | |_| | |
             \ \___/ /
              \_____/";

    public override string EncounterText => "Un Orchetto armato fino ai denti ti blocca la strada!";
}
