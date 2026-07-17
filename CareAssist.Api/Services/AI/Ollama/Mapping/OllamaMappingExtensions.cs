using CareAssist.Api.Contracts.AI;
using CareAssist.Api.Services.AI.Ollama.Contracts;

namespace CareAssist.Api.Services.AI.Ollama.Mapping;

public static class OllamaMappingExtensions
{
    public static List<OllamaMessage> ToOllamaMessages(this IEnumerable<ChatMessage> message)
    {
        return message.Select(m => new OllamaMessage
        (
            m.Role,
            m.Content
        )).ToList();
    }
}
