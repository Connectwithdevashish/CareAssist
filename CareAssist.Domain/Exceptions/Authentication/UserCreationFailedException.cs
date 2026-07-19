namespace CareAssist.Domain.Exceptions.Authentication;

public sealed class UserCreationFailedException : AuthenticationException
{
    public UserCreationFailedException(string message) : base(message)
    {
    }

    public UserCreationFailedException(string message, 
        Exception innerException) : base(message, innerException)
    {
    }
}
