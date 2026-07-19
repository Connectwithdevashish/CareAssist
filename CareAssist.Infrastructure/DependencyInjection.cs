using CareAssist.Application.Abstractions;
using CareAssist.Application.Abstractions.Authentication;
using CareAssist.Application.Abstractions.Persistence;
using CareAssist.Domain.Identity;
using CareAssist.Infrastructure.AI.Ollama;
using CareAssist.Infrastructure.Authentication;
using CareAssist.Infrastructure.Extensions;
using CareAssist.Infrastructure.Persistence;
using CareAssist.Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareAssist.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            option => option.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")));

        // Identity and security services
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Add application services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IApplicationContextService>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddHttpContextAccessor();

        // Add Extension methods
        services.AddApplicationHealthChecks();
        services.AddAI(configuration);
        services.AddJwtAuthentication(configuration);

        return services;
    }
}
