using DungeonQuest.Models;

namespace DungeonQuest.Interfaces;

public interface IExporter
{
    string FormatName { get; }
    string FileExtension { get; }
    void Export(IEnumerable<Weapon> weapons, string filePath);
}
