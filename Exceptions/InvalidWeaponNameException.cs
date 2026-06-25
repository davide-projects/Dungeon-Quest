// InvalidWeaponNameException.cs
namespace DungeonQuest.Exceptions;

// Gestisco più eccezioni custom
// per differenziare i messaggi sulle eccezioni
public class InvalidWeaponNameException : InvalidWeaponException
{
    public InvalidWeaponNameException(string message) :
        base(message) { }
}