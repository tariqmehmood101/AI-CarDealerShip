using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarDealership.IntegrationTests;

public class SwaggerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SwaggerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SwaggerJson_IsAvailable_InDevelopment()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("openapi", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CarDealership", content, StringComparison.OrdinalIgnoreCase);
    }
}
