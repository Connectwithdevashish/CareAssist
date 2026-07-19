namespace CareAssist.Infrastructure.AI.Ollama.Contracts;

public sealed record OllamaChatResponse(
    OllamaMessage Message,
    string Model);
