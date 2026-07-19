namespace CareAssist.Domain.Exceptions.Authentication;

public sealed class UserAlreadyExistsException : AuthenticationException
{
    public UserAlreadyExistsException(string message) : base(message)
    {
    }

    public UserAlreadyExistsException(string message, 
        Exception innerException) : base(message, innerException)
    {
    }
}
