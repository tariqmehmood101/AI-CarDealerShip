using CarDealership.Api.Models;
using CarDealership.Api.Persistence;
using CarDealership.Api.Persistence.Interceptors;
using CarDealership.Api.Services;
using CarDealership.Api.Shared.Common;
using CarDealership.Api.Shared.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(new DateTrackingSaveChangesInterceptor());
});

builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var appName = builder.Configuration["App:Name"] ?? "CarDealership DMV Automation API";
var appVersion = builder.Configuration["App:Version"] ?? "1.0.0";
var appEnvironment = builder.Environment.EnvironmentName;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed development data
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.SeedDevelopmentDataAsync();
    }
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseCors();
app.UseHttpsRedirection();

app.MapGet("/health", () =>
{
    var response = new HealthResponse(
        Status: "Healthy",
        Application: appName,
        Version: appVersion,
        Environment: appEnvironment,
        TimestampUtc: DateTime.UtcNow.ToString("o")
    );

    return Results.Ok(response);
})
.WithName("Health")
.WithOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/test/error", (HttpContext _) => throw new InvalidOperationException("Unhandled error test"))
        .WithName("UnhandledErrorTest")
        .WithOpenApi();

    // Bootstrap endpoint (development only) - internal use for test data setup
    app.MapPost("/api/tenants/bootstrap", async (CreateTenantRequest request, ITenantService tenantService) =>
    {
        var result = await tenantService.CreateTenantAsync(request);

        if (result.IsSuccess)
        {
            return Results.Created($"/api/tenants/{result.Value!.TenantId}", result.Value);
        }

        return Results.BadRequest(new { error = result.Error });
    })
    .WithName("BootstrapTenant")
    .WithOpenApi()
    .WithDescription("Internal development endpoint for creating test tenants with both Sandbox and Production environments.");
}

// Current tenant endpoint (placeholder for future auth integration)
app.MapGet("/api/tenants/current", () =>
{
    // Placeholder: will be populated after auth implementation
    return Results.NotFound();
})
.WithName("GetCurrentTenant")
.WithOpenApi()
.WithDescription("Returns the current tenant context. Requires authentication (not yet implemented).");

app.Run();

public partial class Program { }
