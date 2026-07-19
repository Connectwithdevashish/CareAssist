using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CareAssist.Infrastructure.AI.Fake;

public sealed class FakeHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("Fake AI service is healthy."));
    }
}
