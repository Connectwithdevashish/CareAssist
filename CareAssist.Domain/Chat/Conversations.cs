using CareAssist.Domain.BaseEntities;
using CareAssist.Domain.Identity;

namespace CareAssist.Domain.Chat;

public sealed class Conversations : BaseEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "New Coversation";
    public string UserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
