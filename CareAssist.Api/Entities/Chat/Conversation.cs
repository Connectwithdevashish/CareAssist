using CareAssist.Api.Entities.BaseEntities;
using CareAssist.Api.Entities.Identity;

namespace CareAssist.Api.Entities.Chat;

public sealed class Conversation : BaseEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "New Coversation";
    public string UserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
