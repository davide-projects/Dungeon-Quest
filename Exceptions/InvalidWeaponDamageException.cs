namespace DungeonQuest.Exceptions;

public class InvalidWeaponDamageException : InvalidWeaponException
{
    public InvalidWeaponDamageException (string message) :
        base(message) { }
}