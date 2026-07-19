namespace CareAssist.Domain.Exceptions.Conversations;

public sealed class ConversationAccessDeniedException : ConversationException
{
    public ConversationAccessDeniedException(string message) : base(message)
    {
    }

    public ConversationAccessDeniedException(string message, 
        Exception innerException) : base(message, innerException)
    {
    }
}
