using CareAssist.Api.Extensions;
using System.Diagnostics;

namespace CareAssist.Api.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger, 
        RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            string? userId = null;

            var user = context.User;

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                userId = user.GetUserId();
            }

            var statusCode = context.Response.StatusCode;

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms. UserId: {UserId}. TraceId: {TraceIdentifier}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    userId,
                    context.TraceIdentifier);
        }
    }
}
