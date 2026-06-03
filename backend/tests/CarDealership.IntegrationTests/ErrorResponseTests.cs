using System.Net;
using System.Net.Http.Json;
using CarDealership.Api.Shared.Common;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarDealership.IntegrationTests;

public class ErrorResponseTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ErrorResponseTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UnhandledException_ReturnsStandardErrorShape()
    {
        var response = await _client.GetAsync("/test/error");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal(500, payload?.Status);
        Assert.Equal("Internal Server Error", payload?.Title);
        Assert.False(string.IsNullOrWhiteSpace(payload?.TraceId));
        Assert.Contains("unexpected error", payload?.Detail, StringComparison.OrdinalIgnoreCase);
    }
}
