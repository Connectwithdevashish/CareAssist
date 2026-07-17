namespace CareAssist.Api.Services.AI.Ollama.Contracts;

public sealed record OllamaChatRequest(
    string Model,
    IEnumerable<OllamaMessage> Messages,
    bool Stream = false)
{
}
