namespace DungeonQuest.Models;

public class UrukHai : Enemy
{
    private const int HealAmount = 10;

    public UrukHai()
        : base("Uruk-hai", 32, 9, 22, 50, new CriticalStrikeBehavior()) { }

    public override double PotionDropChance => 0.35;
    public override double XpBoostPotionDropChance => 0.15;

    public override string AsciiArt => @"
             .   /|   .
            / \ / | / \
           | .-. || .-. |
           | |_| || |_| |
            \__/\|/\__/
           _/___/|\___\_
          |  ___   ___  |
          | |_ _| |_ _| |
          |  (_)   (_)  |
           \___________/";

    public override string EncounterText => "Un imponente Uruk-hai avanza minaccioso, pronto a farti a pezzi!";

    public override string OnDefeated(Hero hero)
    {
        hero.Heal(HealAmount);
        return $"{hero.Name} recupera {HealAmount} HP dopo la faticosa vittoria!";
    }
}
