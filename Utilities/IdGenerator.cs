namespace DungeonQuest.Utilities;


// Statica perchè sarà solo utilizzata mai istanziata
public static class IdGenerator
{
    private static int _nextWeaponId = 1;
    private static int _nextSpellId = 1;

    public static int NextWeaponId()
    {
        return _nextWeaponId++;
    }

    public static int NextSpellId()
    {
        return _nextSpellId++;
    }

    public static void SetNextId(int nextId)
    {
        _nextWeaponId = nextId;
    }
}