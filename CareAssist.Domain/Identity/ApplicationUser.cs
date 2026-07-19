using Microsoft.AspNetCore.Identity;

namespace CareAssist.Domain.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public DateTime CreatedAtUtc { get; set; }
    public bool IsActive { get; set; }
}
