using CareAssist.Domain.BaseEntities;
using CareAssist.Domain.Enum;

namespace CareAssist.Domain.Chat;

public sealed class Message : BaseEntity
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Conversations Conversation { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageRole Role { get; set; }
}
