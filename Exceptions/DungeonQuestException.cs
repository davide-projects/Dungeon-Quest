namespace DungeonQuest.Exceptions;


// Astratta per evitare di istanziare questa eccezione
public abstract class DungeonQuestException : Exception
{
    protected  DungeonQuestException(string message) : 
        base(message)
    {}
}