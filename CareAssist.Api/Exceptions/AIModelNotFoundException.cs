namespace CareAssist.Api.Exceptions;

public class AIModelNotFoundException : AIException
{
    private readonly string ProviderName;
    public AIModelNotFoundException(string message,
        string providerName) : base(message, providerName)
    {
        ProviderName = providerName;
    }

    public AIModelNotFoundException(string message,
        Exception innerexception,
        string providerName) : base(message, innerexception, providerName)
    {
        ProviderName = providerName;
    }
}
