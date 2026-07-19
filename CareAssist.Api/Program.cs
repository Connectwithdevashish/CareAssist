using CareAssist.Api.Extensions;
using CareAssist.Application;
using CareAssist.Application.Abstractions;
using CareAssist.Application.Abstractions.Authentication;
using CareAssist.Application.Abstractions.Persistence;
using CareAssist.Domain.Identity;
using CareAssist.Infrastructure.AI.Ollama;
using CareAssist.Infrastructure.Authentication;
using CareAssist.Infrastructure.Extensions;
using CareAssist.Infrastructure.Persistence;
using CareAssist.Infrastructure.Persistence.ContextFile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, service, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(service);
    });

    // Health checkup
    builder.Services.AddApplicationHealthChecks();

    // Framework services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerDocumentation();


    // Infrastructure services
    builder.Services.AddDbContext<ApplicationDbContext>(
        option => option.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));


    // Identity and security services
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddJwtAuthentication(
        builder.Configuration);


    // Application services
    builder.Services.AddValidation();
    builder.Services.AddAI(builder.Configuration);
    builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
    builder.Services.AddApplication();
    builder.Services.AddScoped<IApplicationContextService>(provider =>
        provider.GetRequiredService<ApplicationDbContext>());
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, HttpContextServiceFile>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseInfrastructureMiddleware();

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/health");

    app.Run();

}
catch(Exception ex)
{
    Log.Fatal(ex, "An error occurred while running the application.");
}
finally
{
    Log.CloseAndFlush();
}
