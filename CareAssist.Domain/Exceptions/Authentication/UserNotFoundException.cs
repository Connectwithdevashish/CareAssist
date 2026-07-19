namespace CareAssist.Domain.Exceptions.Authentication;

public class UserNotFoundException : AuthenticationException
{
    public UserNotFoundException(string message) : base(message)
    {
    }

    public UserNotFoundException(string message, 
        Exception innerException) : base(message, innerException)
    {
    }
}
