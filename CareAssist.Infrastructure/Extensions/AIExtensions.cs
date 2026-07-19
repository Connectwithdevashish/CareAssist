using CareAssist.Api.Configuration.AI;
using CareAssist.Application.Abstractions.AI;
using CareAssist.Infrastructure.AI;
using CareAssist.Infrastructure.AI.Fake;
using CareAssist.Infrastructure.AI.Ollama;
using CareAssist.Infrastructure.AI.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CareAssist.Infrastructure.Extensions;

public static class AIExtensions
{
    public static IServiceCollection AddAI(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AIOptions>
            (configuration.GetSection(AIOptions.SectionName));

        var options = configuration.GetSection(AIOptions.SectionName).Get<AIOptions>()
            ?? throw new InvalidOperationException(
                "AI configuration section is missing.");

        if (!Enum.TryParse<AIProvider>(options.Provider?.Trim(), ignoreCase: true, out var provider))
        {
            throw new InvalidOperationException($"Unsupported AI provider: {options.Provider}");
        }

        switch (provider)
        {
            case AIProvider.ollama:
                RegisterOllama(services);
                break;
            case AIProvider.fake:
                RegisterFake(services);
                break;
            default:
                throw new InvalidOperationException($"Unsupported AI provider: {options.Provider}");
        }

        return services;
    }

    private static void RegisterFake(IServiceCollection services)
    {
        services.AddScoped<IChatCompletionService, FakeChatCompletionService>();

        services.AddHealthChecks()
            .AddCheck<FakeHealthCheck>("fake-ai");
    }

    private static void RegisterOllama(IServiceCollection services)
    {
        services.AddHttpClient<OllamaHealthCheck>(ConfigureOllamaClient);

        services.AddHttpClient<IChatCompletionService, OllamaChatCompletionService>(ConfigureOllamaClient);

        services.AddHealthChecks()
            .AddCheck<OllamaHealthCheck>("ollama-ai");
    }

    private static void ConfigureOllamaClient(IServiceProvider serviceprovider, HttpClient httpClient)
    {
        var aiOptions = serviceprovider.GetRequiredService<IOptions<AIOptions>>().Value;

        httpClient.BaseAddress = new Uri(aiOptions.BaseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(aiOptions.TimeoutInSeconds);
    }
}
