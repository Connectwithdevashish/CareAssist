namespace CareAssist.Infrastructure.Exceptions;

public abstract class AIException : Exception
{
    private readonly string ProviderName;
    protected AIException(string message,
        string providerName) : base(message)
    {
        ProviderName = providerName;
    }

    protected AIException(string message,
        Exception innerexception,
        string providerName) : base(message, innerexception)
    {
        ProviderName = providerName;
    }
}
