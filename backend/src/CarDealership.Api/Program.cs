using CarDealership.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
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

app.Run();

public partial class Program { }
