using CareAssist.Api.Extensions;
using CareAssist.Application;
using CareAssist.Infrastructure;
using CareAssist.Infrastructure.Extensions;
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
        .ReadFrom.Services(service)
        .Enrich.FromLogContext();
    });

    // Framework services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerDocumentation();

    // Application services
    builder.Services.AddApplication();

    // Infrastructure services
    builder.Services.AddInfrastructure(
        builder.Configuration);

    Log.Information("Before Build");
    var app = builder.Build();

    Log.Information("Before ApplyMigrationsAsync");
    await app.ApplyMigrationsAsync();

    // Configure the HTTP request pipeline
    Log.Information("Before UseSwagger");
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    Log.Information("Before UseInfrastructureMiddleware");
    app.UseInfrastructureMiddleware();

    Log.Information("Before UseSerilogRequestLogging");
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    Log.Information("Before UseAuthentication");
    app.UseAuthentication();

    Log.Information("Before UseAuthorization");
    app.UseAuthorization();

    Log.Information("Before MapControllers");
    app.MapControllers();

    Log.Information("Before MapHealthChecks");
    app.MapHealthChecks("/health");

    Log.Information("Before Run");
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
