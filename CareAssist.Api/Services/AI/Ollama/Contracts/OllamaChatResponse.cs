namespace CareAssist.Api.Services.AI.Ollama.Contracts;

public sealed record OllamaChatResponse(
    OllamaMessage Message,
    string Model)
{
}
