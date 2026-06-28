namespace DungeonQuest.UI;

public class Welcome
{
    public static string AskName()
    {
        string heroName;
        do
        {
            Console.Write("Benvenuto in Dungeon Quest! Inserisci il nome del tuo eroe: ");
            heroName = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(heroName))
                Console.WriteLine("Devi dare il nome al tuo Eroe. Riprova.");
        } while (string.IsNullOrWhiteSpace(heroName));

        return GraphicsHelper.Capitalize(heroName);
    }
}
