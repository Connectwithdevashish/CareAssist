using CareAssist.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CareAssist.Api.Middleware.ExceptionHandling;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, 
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "An unhandled exception occurred while processing the request. Path: {Path}, Method: {Method}",
                context.Request.Path,
                context.Request.Method);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context,
        Exception exception)
    {
        var metadata = ExceptionMapper.MapException(exception);

        context.Response.StatusCode = (int)metadata.StatusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Type = metadata.ProblemType,
            Status = context.Response.StatusCode,
            Title = metadata.Title,
            Detail = exception switch
            {
                AIException => exception.Message,
                _ => "Please contact support if the problem persists."
            },
            Instance = context.Request.Path,
            Extensions =
            {
                ["traceId"] = context.TraceIdentifier
            }
        };

        await context.Response.WriteAsJsonAsync(problemDetails, 
            cancellationToken: context.RequestAborted);
    }
}
