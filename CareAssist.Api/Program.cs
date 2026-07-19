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
        .ReadFrom.Services(service);
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


    var app = builder.Build();

    await app.ApplyMigrationsAsync();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseInfrastructureMiddleware();

    app.UseSerilogRequestLogging();

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
