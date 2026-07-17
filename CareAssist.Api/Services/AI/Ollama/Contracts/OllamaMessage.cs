namespace CareAssist.Api.Services.AI.Ollama.Contracts;

public sealed record OllamaMessage(
    string Role,
    string Content)
{
}
