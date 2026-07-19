namespace CareAssist.Domain.Exceptions.Authentication;

public class UnauthorizedUserAccessException : AuthenticationException
{
    public UnauthorizedUserAccessException(string message) : base(message)
    {
    }

    public UnauthorizedUserAccessException(string message, 
        Exception innerException) : base(message, innerException)
    {
    }
}
