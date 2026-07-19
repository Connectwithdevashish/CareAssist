namespace CareAssist.Domain.Exceptions.Messages;

public sealed class MessageNotFoundException : MessageException
{
    public MessageNotFoundException(string message) : base(message)
    {
    }

    public MessageNotFoundException(string message, 
        Exception innerException) : base(message, innerException)
    {
    }
}
