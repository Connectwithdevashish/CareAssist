namespace CareAssist.Domain.Exceptions.Messages;

public sealed class MessageAccessDeniedException : MessageException
{
    public MessageAccessDeniedException(string message) : base(message)
    {
    }

    public MessageAccessDeniedException(string message, 
        Exception innerException) : base(message, innerException)
    {
    }
}
