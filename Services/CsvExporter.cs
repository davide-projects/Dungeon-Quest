using DungeonQuest.Exceptions;
using DungeonQuest.Interfaces;
using DungeonQuest.Models;

namespace DungeonQuest.Services;

public class CsvExporter : IExporter
{
    public string FormatName => "CSV";
    public string FileExtension => ".csv";

    public void Export(IEnumerable<Weapon> weapons, string filePath)
    {
        ArgumentNullException.ThrowIfNull(weapons);

        try
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("Id;Nome;Tipo;Danno;Rarit\u00e0;");

            foreach (var weapon in weapons)
                writer.WriteLine($"{weapon.Id};{weapon.Name};{weapon.Type};{weapon.Damage};{weapon.Rarity};");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new ExportException($"Errore di scrittura del file CSV '{filePath}': {ex.Message}", ex);
        }
    }
}
