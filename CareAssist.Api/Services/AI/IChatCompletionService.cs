using CareAssist.Api.Contracts.AI;

namespace CareAssist.Api.Services.AI;

public interface IChatCompletionService
{
    public Task<ChatResponse> GenerateResponseAsync(IEnumerable<ChatMessage> messages, 
        CancellationToken cancellationToken = default);
}
