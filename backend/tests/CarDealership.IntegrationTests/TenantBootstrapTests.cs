using System.Net;
using System.Net.Http.Json;
using CarDealership.Api.Models;
using CarDealership.Api.Persistence;
using CarDealership.Api.Shared.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarDealership.IntegrationTests;

public class TenantBootstrapTests : IAsyncLifetime
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext _dbContext;

    public TenantBootstrapTests()
    {
        _factory = new TestWebApplicationFactory();
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _scope.Dispose();
        _client.Dispose();
        _factory.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task BootstrapEndpoint_CreatesTenant_WithBothEnvironments()
    {
        // Arrange
        var request = new CreateTenantRequest("integration-test", "Integration Test Dealership");

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants/bootstrap", request);
        var result = await response.Content.ReadFromJsonAsync<CreateTenantResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("integration-test", result.Slug);
        Assert.Equal("Integration Test Dealership", result.LegalName);
        Assert.Equal("Active", result.Status);

        // Verify both environments were created
        var environments = await _dbContext.TenantEnvironments
            .Where(e => e.TenantId == result.TenantId)
            .ToListAsync();

        Assert.Equal(2, environments.Count);
    }

    [Fact]
    public async Task BootstrapEndpoint_CreatesTenantSettings_ForBothEnvironments()
    {
        // Arrange
        var request = new CreateTenantRequest("settings-test", "Settings Test Dealership");

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants/bootstrap", request);
        var result = await response.Content.ReadFromJsonAsync<CreateTenantResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(result);

        var settings = await _dbContext!.TenantSettings
            .Where(s => s.TenantId == result.TenantId)
            .ToListAsync();

        Assert.Equal(2, settings.Count);
        Assert.All(settings, s =>
        {
            Assert.Equal("Settings Test Dealership", s.LegalName);
            Assert.False(s.FeatureBillingEnabled);
        });
    }

    [Fact]
    public async Task BootstrapEndpoint_Rejects_DuplicateSlug()
    {
        // Arrange
        var request1 = new CreateTenantRequest("duplicate-slug", "First Dealership");
        await _client.PostAsJsonAsync("/api/tenants/bootstrap", request1);

        var request2 = new CreateTenantRequest("duplicate-slug", "Different Dealership");

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants/bootstrap", request2);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var contentBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("already exists", contentBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task BootstrapEndpoint_Rejects_DuplicateLegalName()
    {
        // Arrange
        var request1 = new CreateTenantRequest("slug-1", "Duplicate Name LLC");
        await _client.PostAsJsonAsync("/api/tenants/bootstrap", request1);

        var request2 = new CreateTenantRequest("slug-2", "Duplicate Name LLC");

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants/bootstrap", request2);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BootstrapEndpoint_Rejects_MissingSlug()
    {
        // Arrange
        var request = new CreateTenantRequest("", "Test Dealership");

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants/bootstrap", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BootstrapEndpoint_Rejects_MissingLegalName()
    {
        // Arrange
        var request = new CreateTenantRequest("test-slug", "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants/bootstrap", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BootstrapEndpoint_Isolates_SandboxAndProduction()
    {
        // Arrange
        var request = new CreateTenantRequest("isolation-test", "Isolation Test");

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants/bootstrap", request);
        var result = await response.Content.ReadFromJsonAsync<CreateTenantResponse>();

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(result!.SandboxEnvironmentId, result.ProductionEnvironmentId);

        var sandboxSettings = await _dbContext!.TenantSettings
            .FirstAsync(s => s.TenantId == result.TenantId && s.Environment == TenantEnvironment.Sandbox);

        var productionSettings = await _dbContext.TenantSettings
            .FirstAsync(s => s.TenantId == result.TenantId && s.Environment == TenantEnvironment.Production);

        Assert.NotEqual(sandboxSettings.Id, productionSettings.Id);
    }

    [Fact]
    public async Task BootstrapEndpoint_IsDisabled_OutsideDevelopment()
    {
        // Arrange
        using var productionFactory = new TestWebApplicationFactory("Production");
        using var productionClient = productionFactory.CreateClient();
        var request = new CreateTenantRequest("prod-test", "Production Test Dealership");

        // Act
        var response = await productionClient.PostAsJsonAsync("/api/tenants/bootstrap", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CurrentTenantEndpoint_ReturnsNotFound_BeforeAuthImplemented()
    {
        // Act
        var response = await _client.GetAsync("/api/tenants/current");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
