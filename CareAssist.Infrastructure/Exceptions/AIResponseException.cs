using System.Net;

namespace CareAssist.Infrastructure.Exceptions;

public sealed class AIResponseException : AIException
{
    private HttpStatusCode _statusCode;
    private readonly string ProviderName;
    public AIResponseException(string message,
        HttpStatusCode statusCode,
        string providerName) : base(message, providerName)
    {
        _statusCode = statusCode;
        ProviderName = providerName;
    }

    public AIResponseException(string message,
        Exception innerexception,
        HttpStatusCode statusCode,
        string providerName) : base(message, innerexception, providerName)
    {
        _statusCode = statusCode;
        ProviderName = providerName;
    }
}