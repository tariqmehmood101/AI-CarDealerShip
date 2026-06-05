using CarDealership.Api.Models;
using CarDealership.Api.Persistence;
using CarDealership.Api.Services;
using CarDealership.Api.Shared.Common;
using CarDealership.Api.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.UnitTests;

public class TenantServiceTests : IAsyncLifetime
{
    private ApplicationDbContext _dbContext;
    private ITenantService _tenantService;

    public Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _tenantService = new TenantService(_dbContext);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task CreateTenantAsync_Creates_Tenant_With_Both_Environments()
    {
        // Arrange
        var request = new CreateTenantRequest("test-tenant", "Test Dealership LLC");

        // Act
        var result = await _tenantService.CreateTenantAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("test-tenant", result.Value.Slug);
        Assert.Equal("Test Dealership LLC", result.Value.LegalName);
        Assert.Equal("Active", result.Value.Status);

        // Verify both environments exist
        var environments = await _dbContext.TenantEnvironments
            .Where(e => e.TenantId == result.Value.TenantId)
            .ToListAsync();

        Assert.Equal(2, environments.Count);
        Assert.Contains(environments, e => e.Environment == TenantEnvironment.Sandbox);
        Assert.Contains(environments, e => e.Environment == TenantEnvironment.Production);
    }

    [Fact]
    public async Task CreateTenantAsync_Creates_TenantSettings_For_Both_Environments()
    {
        // Arrange
        var request = new CreateTenantRequest("test-tenant-2", "Test Dealership 2 LLC");

        // Act
        var result = await _tenantService.CreateTenantAsync(request);

        // Assert
        Assert.True(result.IsSuccess);

        var settings = await _dbContext.TenantSettings
            .Where(s => s.TenantId == result.Value!.TenantId)
            .ToListAsync();

        Assert.Equal(2, settings.Count);
        Assert.All(settings, s =>
        {
            Assert.Equal("Test Dealership 2 LLC", s.LegalName);
            Assert.False(s.FeatureBillingEnabled);
            Assert.False(s.FeatureDocuSignEnabled);
            Assert.False(s.FeatureInStoreSignaturesEnabled);
            Assert.False(s.FeatureFutureAIEnabled);
            Assert.False(s.FeatureDmvSubmissionEnabled);
        });

        var sandboxSettings = settings.FirstOrDefault(s => s.Environment == TenantEnvironment.Sandbox);
        var productionSettings = settings.FirstOrDefault(s => s.Environment == TenantEnvironment.Production);

        Assert.NotNull(sandboxSettings);
        Assert.NotNull(productionSettings);
        Assert.NotEqual(sandboxSettings.Id, productionSettings.Id);
    }

    [Fact]
    public async Task CreateTenantAsync_Fails_With_Duplicate_Slug()
    {
        // Arrange
        var request = new CreateTenantRequest("duplicate-slug", "First Dealership");
        await _tenantService.CreateTenantAsync(request);

        // Act
        var duplicateRequest = new CreateTenantRequest("duplicate-slug", "Different Dealership");
        var result = await _tenantService.CreateTenantAsync(duplicateRequest);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("already exists", result.Error);
    }

    [Fact]
    public async Task CreateTenantAsync_Fails_With_Duplicate_LegalName()
    {
        // Arrange
        var request = new CreateTenantRequest("slug-1", "Duplicate Name LLC");
        await _tenantService.CreateTenantAsync(request);

        // Act
        var duplicateRequest = new CreateTenantRequest("slug-2", "Duplicate Name LLC");
        var result = await _tenantService.CreateTenantAsync(duplicateRequest);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("already exists", result.Error);
    }

    [Fact]
    public async Task CreateTenantAsync_Fails_With_Missing_Slug()
    {
        // Arrange
        var request = new CreateTenantRequest("", "Test Dealership");

        // Act
        var result = await _tenantService.CreateTenantAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("slug", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateTenantAsync_Fails_With_Missing_LegalName()
    {
        // Arrange
        var request = new CreateTenantRequest("test-slug", "");

        // Act
        var result = await _tenantService.CreateTenantAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("legal name", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateTenantAsync_Trims_Whitespace_From_Input()
    {
        // Arrange
        var request = new CreateTenantRequest("  test-slug  ", "  Test Dealership  ");

        // Act
        var result = await _tenantService.CreateTenantAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("test-slug", result.Value!.Slug);
        Assert.Equal("Test Dealership", result.Value.LegalName);
    }

    [Fact]
    public async Task CreateTenantAsync_Sets_Default_Status_To_Active()
    {
        // Arrange
        var request = new CreateTenantRequest("test-status", "Status Test");

        // Act
        var result = await _tenantService.CreateTenantAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Active", result.Value!.Status);
    }
}
