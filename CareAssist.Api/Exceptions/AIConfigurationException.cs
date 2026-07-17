namespace CareAssist.Api.Exceptions;

public class AIConfigurationException : AIException
{
    private readonly string ProviderName;
    public AIConfigurationException(string message,
        string providerName) : base(message, providerName)
    {
        ProviderName = providerName;
    }

    public AIConfigurationException(string message,
        Exception innerexception,
        string providerName) : base(message, innerexception, providerName)
    {
        ProviderName = providerName;
    }
}
