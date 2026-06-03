using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CarDealership.Api.Shared.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var correlationId = context.Items["X-Correlation-ID"]?.ToString() ?? "unknown";

        _logger.LogInformation("Incoming request {Method} {Path}{QueryString} | CorrelationId={CorrelationId}",
            request.Method,
            request.Path,
            request.QueryString,
            correlationId);

        await _next(context);

        _logger.LogInformation("Completed request {Method} {Path} with {StatusCode} | CorrelationId={CorrelationId}",
            request.Method,
            request.Path,
            context.Response.StatusCode,
            correlationId);
    }
}
