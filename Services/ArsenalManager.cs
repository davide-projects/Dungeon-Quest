using DungeonQuest.db;
using DungeonQuest.Exceptions;
using DungeonQuest.Models;
using DungeonQuest.UI;
using Microsoft.EntityFrameworkCore;

namespace DungeonQuest.Services;

public class ArsenalManager
{
    private readonly DungeonContext _db;
    private readonly Hero _hero;

    public ArsenalManager(DungeonContext db, Hero hero)
    {
        _db = db;
        _hero = hero;
    }

    public int PotionCount => _db.Potions.Count(p => p.HeroId == _hero.Id);

    public List<Potion> Potions => _db.Potions
        .Where(p => p.HeroId == _hero.Id)
        .OrderBy(p => p.Id)
        .ToList();

    public void AddPotion(Potion potion)
    {
        potion.HeroId = _hero.Id;
        _db.Potions.Add(potion);
        _db.SaveChanges();
    }

    public bool TryUsePotion()
    {
        var potion = _db.Potions
            .Where(p => p.HeroId == _hero.Id)
            .OrderBy(p => p.Id)
            .FirstOrDefault();

        if (potion is null)
            return false;

        if (_hero.Hp >= _hero.MaxHp)
        {
            GraphicsHelper.WriteCombatAction($"{_hero.Name} è già in piena forma! Usare una pozione ora sarebbe uno spreco.", ConsoleColor.DarkYellow);
            return false;
        }

        var amount = _hero.MaxHp / 2;
        _hero.Heal(amount);
        _db.Potions.Remove(potion);
        _db.SaveChanges();
        Console.WriteLine($"   {_hero.Name} usa una pozione e recupera {amount} punti vita!");
        return true;
    }

    public void AddWeapon(Weapon weapon)
    {
        weapon.HeroId = _hero.Id;
        _db.Weapons.Add(weapon);
        _db.SaveChanges();
    }

    public void AddWeapon(string name, WeaponType type, int damage, WeaponRarity rarity = WeaponRarity.Common)
    {
        var weapon = new Weapon(name, type, damage, rarity)
        {
            HeroId = _hero.Id
        };
        _db.Weapons.Add(weapon);
        _db.SaveChanges();
    }

    public List<Weapon> GetAllWeapons()
    {
        return _db.Weapons
            .Where(w => w.HeroId == _hero.Id)
            .OrderBy(w => w.Code)
            .ToList();
    }

    public List<Weapon> GetWeaponsByType(WeaponType type)
    {
        return _db.Weapons
            .Where(w => w.HeroId == _hero.Id && w.Type == type)
            .OrderBy(w => w.Code)
            .ToList();
    }

    public Weapon FindByName(string name)
    {
        var weapon = _db.Weapons.FirstOrDefault(w =>
            w.HeroId == _hero.Id && EF.Functions.Like(w.Name, $"%{name}%"));

        if (weapon is null)
            throw new WeaponNotFoundException($"Nessuna arma trovata con nome '{name}'.");

        return weapon;
    }

    public List<Weapon> FindAllByName(string name)
    {
        return _db.Weapons
            .Where(w => w.HeroId == _hero.Id && EF.Functions.Like(w.Name, $"%{name}%"))
            .OrderBy(w => w.Code)
            .ToList();
    }

    public bool TryFindByName(string name, out Weapon? weapon)
    {
        weapon = _db.Weapons.FirstOrDefault(w =>
            w.HeroId == _hero.Id && EF.Functions.Like(w.Name, $"%{name}%"));

        return weapon is not null;
    }

    public void EquipByName(string name)
    {
        var weapon = FindByName(name);

        if (_hero.EquippedWeapon is not null)
            _hero.EquippedWeapon.EquippedByHeroId = null;

        weapon.EquippedByHeroId = _hero.Id;
        _hero.EquippedWeapon = weapon;
        _db.SaveChanges();

        Console.WriteLine($"Equipaggiata: {weapon}");
    }

    public void UpdateWeapon(Weapon weapon, string name, WeaponType type, int damage, WeaponRarity rarity)
    {
        var wasEquipped = weapon.EquippedByHeroId == _hero.Id;

        weapon.Update(name, type, damage, rarity);

        if (wasEquipped)
            _hero.EquippedWeapon = weapon;

        _db.SaveChanges();
    }

    public void SaveToCsv(string filePath)
    {
        var weapons = GetAllWeapons();
        using var writer = new StreamWriter(filePath);

        writer.WriteLine("Codice;Nome;Tipo;Danno;Rarità;");

        foreach (var weapon in weapons)
            writer.WriteLine($"{weapon.Code};{weapon.Name};{weapon.Type};{weapon.Damage};{weapon.Rarity};");
    }
}
