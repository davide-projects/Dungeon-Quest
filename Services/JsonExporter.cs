using System.Text.Encodings.Web;
using System.Text.Json;
using DungeonQuest.Exceptions;
using DungeonQuest.Interfaces;
using DungeonQuest.Models;

namespace DungeonQuest.Services;

public class JsonExporter : IExporter
{
    public string FormatName => "JSON";
    public string FileExtension => ".json";

    public void Export(IEnumerable<Weapon> weapons, string filePath)
    {
        ArgumentNullException.ThrowIfNull(weapons);

        try
        {
            var data = weapons.Select(w => new
            {
                w.Id,
                w.Name,
                Tipo = w.Type.DisplayName,
                Danno = w.Damage,
                Rarit\u00e0 = w.Rarity.DisplayName
            });

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            File.WriteAllText(filePath, json);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new ExportException($"Errore di scrittura del file JSON '{filePath}': {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new ExportException($"Errore di serializzazione JSON: {ex.Message}", ex);
        }
    }
}
