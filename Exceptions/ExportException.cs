namespace DungeonQuest.Exceptions;

public class ExportException : DungeonQuestException
{
    public ExportException(string message) : base(message) { }

    public ExportException(string message, Exception innerException) : base(message) { }
}
