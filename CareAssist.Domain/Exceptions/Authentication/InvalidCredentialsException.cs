namespace CareAssist.Domain.Exceptions.Authentication;

public sealed class InvalidCredentialsException : AuthenticationException
{
    public InvalidCredentialsException(string message) : base(message)
    {
    }

    public InvalidCredentialsException(string message, 
        Exception innerException) : base(message, innerException)
    {
    }
}
