namespace DungeonQuest.Exceptions;

public class WeaponNotFoundException : DungeonQuestException
{
    public WeaponNotFoundException (string message) :
        base(message) {}
}