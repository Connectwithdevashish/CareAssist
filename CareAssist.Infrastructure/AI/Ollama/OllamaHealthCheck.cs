using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CareAssist.Infrastructure.AI.Ollama;

public class OllamaHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    public OllamaHealthCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags", cancellationToken);

            if (response.IsSuccessStatusCode)
                return Healthy();

            return Unhealthy();
        }
        catch (Exception)
        {
            return Unhealthy();
        }
    }

    private HealthCheckResult Unhealthy()
    {
        return HealthCheckResult.Unhealthy("Ollama AI service is unavailable.");
    }

    private HealthCheckResult Healthy()
    {
        return HealthCheckResult.Healthy("Ollama AI service is healthy.");
    }
}
