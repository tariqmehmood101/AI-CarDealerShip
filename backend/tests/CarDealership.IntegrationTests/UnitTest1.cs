using System.Net;
using System.Net.Http.Json;
using CarDealership.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarDealership.IntegrationTests;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsOkAndHealthResponse()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Healthy", payload?.Status);
        Assert.False(string.IsNullOrWhiteSpace(payload?.Application));
        Assert.False(string.IsNullOrWhiteSpace(payload?.Version));
    }
}
