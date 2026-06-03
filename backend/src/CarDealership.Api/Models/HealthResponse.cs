namespace CarDealership.Api.Models;

public sealed record HealthResponse(
    string Status,
    string Application,
    string Version,
    string Environment,
    string TimestampUtc
);
