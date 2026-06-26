using DungeonQuest.db;
using DungeonQuest.Exceptions;
using DungeonQuest.Models;
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
        var pozione = _db.Potions
            .Where(p => p.HeroId == _hero.Id)
            .OrderBy(p => p.Id)
            .FirstOrDefault();

        if (pozione is null)
            return false;

        var amount = _hero.MaxHp / 2;
        _hero.Heal(amount);
        _db.Potions.Remove(pozione);
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

    public void AddWeapon(string name, WeaponType type, int damage, WeaponRarity rarity = WeaponRarity.Comune)
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
        var arma = _db.Weapons.FirstOrDefault(w =>
            w.HeroId == _hero.Id && EF.Functions.Like(w.Name, $"%{name}%"));

        if (arma is null)
            throw new WeaponNotFoundException($"Nessuna arma trovata con nome '{name}'.");

        return arma;
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

    public void TryEquipByName(string name)
    {
        var arma = FindByName(name);

        if (_hero.EquippedWeapon is not null)
            _hero.EquippedWeapon.EquippedByHeroId = null;

        arma.EquippedByHeroId = _hero.Id;
        _hero.EquippedWeapon = arma;
        _db.SaveChanges();

        Console.WriteLine($"Equipaggiata: {arma}");
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
        var armi = GetAllWeapons();
        using var writer = new StreamWriter(filePath);

        writer.WriteLine("Codice;Nome;Tipo;Danno;Rarità;");

        foreach (var arma in armi)
            writer.WriteLine($"{arma.Code};{arma.Name};{arma.Type};{arma.Damage};{arma.Rarity};");
    }
}
