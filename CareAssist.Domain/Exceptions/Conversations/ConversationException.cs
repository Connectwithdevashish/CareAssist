namespace CareAssist.Domain.Exceptions.Conversations;

public class ConversationException : Exception
{
    public ConversationException(string message) : base(message)
    {

    }

    public ConversationException(string message,
        Exception innerException) : base(message, innerException)
    {

    }
}
