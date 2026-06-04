using CarDealership.Api.Models;
using CarDealership.Api.Persistence;
using CarDealership.Api.Persistence.Interceptors;
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
}

app.Run();

public partial class Program { }
