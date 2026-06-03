using CarDealership.Api.Shared.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CarDealership.Api.Shared.Middleware;

public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception caught by global exception handler.");
            await WriteErrorResponseAsync(context, exception);
        }
    }

    private static Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        var traceId = context.Items["X-Correlation-ID"]?.ToString() ?? context.TraceIdentifier;
        var errorResponse = new ApiErrorResponse(
            Type: "https://httpstatuses.com/500",
            Title: "Internal Server Error",
            Status: (int)HttpStatusCode.InternalServerError,
            Detail: "An unexpected error occurred. Please contact support with the correlation ID.",
            TraceId: traceId ?? string.Empty
        );

        var json = JsonSerializer.Serialize(errorResponse);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsync(json);
    }
}
