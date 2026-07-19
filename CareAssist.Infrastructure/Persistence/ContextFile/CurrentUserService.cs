using CareAssist.Application.Abstractions;
using CareAssist.Infrastructure.Persistence.ContextFile.Extensions;
using Microsoft.AspNetCore.Http;

namespace CareAssist.Infrastructure.Persistence.ContextFile;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId => _httpContextAccessor.HttpContext?.User.GetUserId() ?? 
        throw new UnauthorizedAccessException();
}
