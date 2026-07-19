using Microsoft.Extensions.DependencyInjection;

namespace CareAssist.Infrastructure.AI.Ollama;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();

        return services;
    }
}
