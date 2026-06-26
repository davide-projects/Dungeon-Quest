namespace DungeonQuest.Exceptions;

public class InvalidWeaponTypeException : InvalidWeaponException
{
    public InvalidWeaponTypeException(string message) :
        base(message) { }
}
