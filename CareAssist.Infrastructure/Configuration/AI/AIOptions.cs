namespace CareAssist.Infrastructure.Configuration.AI;

public sealed class AIOptions
{
    public const string SectionName = "AI";
    public string Provider { get; init; } = "Fake";
    public int TimeoutInSeconds { get; init; } = 60;
    public string BaseUrl { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
}
