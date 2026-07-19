namespace CareAssist.Domain.Exceptions.Conversations;

public sealed class ConversationNotFoundException : ConversationException
{
    public ConversationNotFoundException(string message) : base(message)
    {
    }

    public ConversationNotFoundException(string message, 
        Exception innerException) : base(message, innerException)
    {
    }
}
