// InvalidWeaponException.cs
namespace DungeonQuest.Exceptions;

public class InvalidWeaponException : DungeonQuestException
{
    public InvalidWeaponException(string message) : base(message) { }
}