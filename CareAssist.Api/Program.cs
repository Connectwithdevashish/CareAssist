using CareAssist.Api.Data;
using CareAssist.Api.Entities.Identity;
using CareAssist.Api.Extensions;
using CareAssist.Api.Services.AI.Ollama;
using CareAssist.Api.Services.Authentication;
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
