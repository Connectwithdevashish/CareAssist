using CareAssist.Infrastructure.Exceptions;
using System.Net;

namespace CareAssist.Api.Middleware.ExceptionHandling;

public static class ExceptionMapper
{
    public static ExceptionMetadata MapException(this Exception exception)
    {
        return exception switch
        {
            AIConfigurationException =>
                new(
                    HttpStatusCode.InternalServerError,
                    "AI configuration error.",
                    "about:blank"),

            AIModelNotFoundException =>
                new(
                    HttpStatusCode.NotFound,
                    "AI model not found.",
                    "about:blank"),

            AIProviderUnavailableException =>
                new(
                    HttpStatusCode.ServiceUnavailable,
                    "AI provider unavailable.",
                    "about:blank"),

            AIRequestTimeoutException =>
                new(
                    HttpStatusCode.GatewayTimeout,
                    "AI request timed out.",
                    "about:blank"),

            AIResponseException =>
                new(
                    HttpStatusCode.BadGateway,
                    "Invalid AI provider response.",
                    "about:blank"),

            _ =>
                new(
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.",
                    "about:blank")
        };
    }
}
