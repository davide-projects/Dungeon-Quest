namespace DungeonQuest.UI;

public class Benvenuto
{
    public static string ChiediNome()
    {
        string nomeEroe;
        do
        {
            Console.Write("Benvenuto in Dungeon Quest! Inserisci il nome del tuo eroe: ");
            nomeEroe = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(nomeEroe))
                Console.WriteLine("Devi dare il nome al tuo Eroe. Riprova.");
        } while (string.IsNullOrWhiteSpace(nomeEroe));

        return char.ToUpper(nomeEroe[0]) + nomeEroe.Substring(1);
    }
}
