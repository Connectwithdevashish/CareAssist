using CareAssist.Domain.Chat;
using Microsoft.EntityFrameworkCore;

namespace CareAssist.Application.Abstractions.Persistence;

public interface IApplicationContextService
{
    DbSet<Conversations> Conversations { get; }
    DbSet<Message> Messages { get; }
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
