namespace CareAssist.Api.Exceptions;

public sealed class AIProviderUnavailableException : AIException
{
    private readonly string ProviderName;
    public AIProviderUnavailableException(string message,
        string providerName) : base(message, providerName)
    {
        ProviderName = providerName;
    }

    public AIProviderUnavailableException(string message,
        Exception innerexception,
        string providerName) : base(message, innerexception, providerName)
    {
        ProviderName = providerName;
    } 
}
