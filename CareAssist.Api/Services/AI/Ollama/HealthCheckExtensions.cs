namespace CareAssist.Api.Services.AI.Ollama;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();

        return services;
    }
}
