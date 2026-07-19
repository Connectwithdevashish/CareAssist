using CareAssist.Contracts.AI;
using CareAssist.Infrastructure.AI.Ollama.Contracts;

namespace CareAssist.Infrastructure.AI.Ollama.Mapping;

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
