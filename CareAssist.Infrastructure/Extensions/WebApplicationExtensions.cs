using CareAssist.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CareAssist.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> ApplyMigrationsAsync(
        this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var pending = await dbContext.Database.GetPendingMigrationsAsync();

            if (!pending.Any())
            {
                logger.LogInformation("Database is already up to date.");
                return app;
            }

            logger.LogInformation(
                "Applying {Count} pending migrations...",
                pending.Count());

            await dbContext.Database.MigrateAsync();

            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex,
                "Failed to apply database migrations.");

            throw;
        }

        return app;
    }
}
