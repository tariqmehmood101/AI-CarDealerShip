using CarDealership.Api.Models;

namespace CarDealership.UnitTests;

public class HealthResponseTests
{
    [Fact]
    public void HealthResponse_ConstructsExpectedValues()
    {
        var response = new HealthResponse(
            Status: "Healthy",
            Application: "CarDealership DMV Automation API",
            Version: "1.0.0",
            Environment: "Development",
            TimestampUtc: DateTime.UtcNow.ToString("o")
        );

        Assert.Equal("Healthy", response.Status);
        Assert.Contains("CarDealership", response.Application);
        Assert.Equal("1.0.0", response.Version);
        Assert.False(string.IsNullOrWhiteSpace(response.TimestampUtc));
    }
}
