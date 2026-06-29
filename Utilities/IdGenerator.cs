namespace DungeonQuest.Utilities;

public static class IdGenerator
{
    private static int _nextWeaponId = 1;

    public static int NextWeaponId()
    {
        return _nextWeaponId++;
    }

    public static void SetNextId(int nextId)
    {
        _nextWeaponId = nextId;
    }
}