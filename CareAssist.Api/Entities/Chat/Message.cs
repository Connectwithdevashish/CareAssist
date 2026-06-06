using CareAssist.Api.Entities.BaseEntities;
using CareAssist.Api.Entities.Enum;

namespace CareAssist.Api.Entities.Chat;

public sealed class Message : BaseEntity
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageRole Role { get; set; }
}
