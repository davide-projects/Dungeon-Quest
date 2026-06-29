namespace DungeonQuest.Models;

public class Sauron : Enemy
{
    public Sauron()
        : base("Sauron", 150, 15, 200, 500, new DarkLordAttackBehavior()) { }

    public override double PotionDropChance => 1.0;

    public override string AsciiArt => @"
             .    /|    .
            / \  / | \ / \
           | .-..--|--..-. |
           | |_||  |  ||_| |
            \_/ |__|__| \_/
           _/_____________\_
          | |  ___   ___  | |
          | | |_ _| |_ _| | |
          | |  (_)   (_)  | |
          | |_____________| |
           \_______________/
             |   |  |   |
             |   |  |   |
            _|   |  |   |_
           /_\__/    \__/_\";

    public override string EncounterText => "L'Oscuro Signore Sauron emerge dalle tenebre! Il destino della Terra di Mezzo è nelle tue mani!";
}
