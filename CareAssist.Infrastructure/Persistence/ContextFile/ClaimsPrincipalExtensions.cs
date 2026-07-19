using System.Security.Claims;

namespace CareAssist.Infrastructure.Persistence.ContextFile.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal User)
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            throw new UnauthorizedAccessException("User ID not found.");
    }
}
