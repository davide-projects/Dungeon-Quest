namespace DungeonQuest.Exceptions;

public class InvalidWeaponRarityException : InvalidWeaponException
{
    public InvalidWeaponRarityException(string message) :
        base(message) { }
}
