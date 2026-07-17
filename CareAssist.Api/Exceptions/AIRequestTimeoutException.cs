namespace CareAssist.Api.Exceptions;

public class AIRequestTimeoutException : AIException
{
    private readonly string ProviderName;
    public AIRequestTimeoutException(string message,
        string providerName) : base(message, providerName)
    {
        ProviderName = providerName;
    }

    public AIRequestTimeoutException(string message,
        Exception innerexception,
        string providerName) : base(message, innerexception, providerName)
    {
        ProviderName = providerName;
    } 
}
