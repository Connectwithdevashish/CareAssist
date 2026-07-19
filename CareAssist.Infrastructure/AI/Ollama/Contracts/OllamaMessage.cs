namespace CareAssist.Infrastructure.AI.Ollama.Contracts;

public sealed record OllamaMessage(
    string Role,
    string Content);
