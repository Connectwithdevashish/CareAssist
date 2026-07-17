using CareAssist.Api.Middleware;
using CareAssist.Api.Middleware.ExceptionHandling;

namespace CareAssist.Api.Extensions;

public static class InfrastructureMiddleware
{
    public static IApplicationBuilder UseInfrastructureMiddleware(
        this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RequestLoggingMiddleware>();
        builder.UseMiddleware<ExceptionHandlingMiddleware>();

        return builder;
    }
}
