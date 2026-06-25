using DungeonQuest.Exceptions;
using DungeonQuest.Models;
using DungeonQuest.Utilities;

namespace DungeonQuest.Services;

public class ArsenalManager
{
    private readonly List<Weapon> _weapons = new();
    private readonly Hero _hero;

    public ArsenalManager(Hero hero)
    {
        _hero = hero;
    }

    public void AddWeapon(Weapon weapon)
    {
        _weapons.Add(weapon);
    }

    public void AddWeapon(string name, WeaponType type, int damage)
    {
        var weapon = new Weapon(name, type, damage);
        _weapons.Add(weapon);
    }

    public List<Weapon> GetAllWeapons()
    {
        return _weapons;
    }

    public List<Weapon> GetWeaponsByType(WeaponType type)
    {
        return _weapons.Where(w => w.Type == type).ToList();
    }

    public Weapon FindByName(string name)
    {
        var arma = _weapons.FirstOrDefault(w =>
            w.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        if (arma is null)
            throw new WeaponNotFoundException($"Nessuna arma trovata con nome '{name}'.");

        return arma;
    }

    public List<Weapon> FindAllByName(string name)
    {
        return _weapons
            .Where(w => w.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public bool TryFindByName(string name, out Weapon? weapon)
    {
        weapon = _weapons.FirstOrDefault(w =>
            w.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        return weapon is not null;
    }

    public void TryEquipByName(string name)
    {
        var arma = FindByName(name);
        _hero.EquippedWeapon = arma;
        Console.WriteLine($"Equipaggiata: {arma}");
    }

    public void SaveToCsv(string filePath)
    {
        using var writer = new StreamWriter(filePath);

        writer.WriteLine("Codice;Nome;Tipo;Danno;");

        foreach (var arma in _weapons)
            writer.WriteLine($"{arma.Code};{arma.Name};{arma.Type};{arma.Damage};");
    }

    public void LoadFromCsv(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        using var reader = new StreamReader(filePath);

        var header = reader.ReadLine();
        if (header is null)
            return;

        int maxCode = 0;

        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split(';');
            if (parts.Length < 4)
                continue;

            // Se l'ultimo campo è vuoto (trailing ;), rimuovilo
            if (parts.Length == 5 && string.IsNullOrEmpty(parts[4]))
                parts = parts.Take(4).ToArray();

            if (parts.Length != 4)
                continue;

            if (!int.TryParse(parts[0], out var code))
                continue;

            var tipo = Enum.Parse<WeaponType>(parts[2]);
            if (!int.TryParse(parts[3], out var danno))
                continue;

            _weapons.Add(new Weapon(code, parts[1], tipo, danno));

            if (code > maxCode)
                maxCode = code;
        }

        IdGenerator.SetNextId(maxCode + 1);
    }
}
