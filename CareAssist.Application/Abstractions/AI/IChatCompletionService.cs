using CareAssist.Contracts.AI;

namespace CareAssist.Application.Abstractions.AI;

public interface IChatCompletionService
{
    public Task<ChatResponse> GenerateResponseAsync(IEnumerable<ChatMessage> messages, 
        CancellationToken cancellationToken = default);
}
