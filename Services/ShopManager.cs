using DungeonQuest.Models;

namespace DungeonQuest.Services;

public class ShopManager
{
    private readonly ArsenalManager _arsenal;
    private readonly Hero _hero;

    public ShopManager(ArsenalManager arsenal, Hero hero)
    {
        _arsenal = arsenal;
        _hero = hero;
    }

    public List<(WeaponTemplate Template, bool Owned)> GetCatalog()
    {
        var ownedNames = _arsenal.GetAllWeapons()
            .Select(w => w.Name)
            .ToHashSet();

        return WeaponCatalog.All
            .Select(t => (t, ownedNames.Contains(t.Name)))
            .ToList();
    }

    public bool CanAfford(WeaponTemplate template)
    {
        return _hero.Gold >= template.Price;
    }

    public bool TryBuy(WeaponTemplate template)
    {
        if (_hero.Gold < template.Price)
            return false;

        _hero.AddReward(-template.Price, 0);
        var weapon = new Weapon(template.Name, template.Type, template.Damage, template.Rarity);
        _arsenal.AddWeapon(weapon);

        return true;
    }

    public WeaponTemplate GetRandomCommon()
    {
        return WeaponCatalog.GetRandomCommon();
    }
}
