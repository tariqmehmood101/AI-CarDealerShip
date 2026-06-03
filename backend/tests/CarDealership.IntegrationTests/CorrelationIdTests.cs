using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarDealership.IntegrationTests;

public class CorrelationIdTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CorrelationIdTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CorrelationId_IsReturnedInResponseHeaders()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var values));
        Assert.False(string.IsNullOrWhiteSpace(values?.FirstOrDefault()));
    }
}
