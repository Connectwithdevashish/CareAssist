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
            logger.LogInformation("Applying database migrations...");

            var db = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            await db.Database.MigrateAsync();

            logger.LogInformation("Database is up to date.");
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
