namespace CareAssist.Contracts.AI;

public sealed record ChatResponse(string Content,
    string Model,
    string? ErrorMessage = null,
    int TokensUsed = 0,
    int PromptTokensUsed = 0);
