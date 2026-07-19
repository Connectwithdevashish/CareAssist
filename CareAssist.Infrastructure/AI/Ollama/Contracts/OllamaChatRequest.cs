namespace CareAssist.Infrastructure.AI.Ollama.Contracts;

public sealed record OllamaChatRequest(
    string Model,
    IEnumerable<OllamaMessage> Messages,
    bool Stream = false);
