namespace CareAssist.Domain.Exceptions.Authentication;

public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message)
    {
        
    }

    public AuthenticationException(string message, 
        Exception innerException) : base(message, innerException)
    {
        
    }
}
