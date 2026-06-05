using CarDealership.Api.Models;
using CarDealership.Api.Persistence;
using CarDealership.Api.Persistence.Entities;
using CarDealership.Api.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Api.Services;

public interface ITenantService
{
    Task<Result<CreateTenantResponse>> CreateTenantAsync(CreateTenantRequest request);
}

public sealed class TenantService : ITenantService
{
    private readonly ApplicationDbContext _dbContext;

    public TenantService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CreateTenantResponse>> CreateTenantAsync(CreateTenantRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Slug))
        {
            return Result<CreateTenantResponse>.Failure("Tenant slug is required.");
        }

        if (string.IsNullOrWhiteSpace(request.LegalName))
        {
            return Result<CreateTenantResponse>.Failure("Tenant legal name is required.");
        }

        // Check for duplicate slug
        var existingBySlug = await _dbContext.Tenants
            .FirstOrDefaultAsync(t => t.Slug == request.Slug.Trim());

        if (existingBySlug != null)
        {
            return Result<CreateTenantResponse>.Failure($"A tenant with slug '{request.Slug}' already exists.");
        }

        // Check for duplicate legal name
        var existingByName = await _dbContext.Tenants
            .FirstOrDefaultAsync(t => t.LegalName == request.LegalName.Trim());

        if (existingByName != null)
        {
            return Result<CreateTenantResponse>.Failure($"A tenant with legal name '{request.LegalName}' already exists.");
        }

        // Create tenant
        var tenant = Tenant.Create(request.Slug.Trim(), request.LegalName.Trim());
        tenant.IsDeleted = false;

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();

        // Create Sandbox environment
        var sandboxEnvironment = new TenantEnvironmentRecord
        {
            TenantId = tenant.Id,
            Environment = TenantEnvironment.Sandbox,
            IsDeleted = false
        };

        _dbContext.TenantEnvironments.Add(sandboxEnvironment);

        // Create Production environment
        var productionEnvironment = new TenantEnvironmentRecord
        {
            TenantId = tenant.Id,
            Environment = TenantEnvironment.Production,
            IsDeleted = false
        };

        _dbContext.TenantEnvironments.Add(productionEnvironment);

        // Create default Sandbox settings
        var sandboxSettings = CreateDefaultTenantSettings(tenant.Id, TenantEnvironment.Sandbox, request.LegalName.Trim());
        sandboxSettings.IsDeleted = false;
        _dbContext.TenantSettings.Add(sandboxSettings);

        // Create default Production settings
        var productionSettings = CreateDefaultTenantSettings(tenant.Id, TenantEnvironment.Production, request.LegalName.Trim());
        productionSettings.IsDeleted = false;
        _dbContext.TenantSettings.Add(productionSettings);

        await _dbContext.SaveChangesAsync();

        return Result<CreateTenantResponse>.Success(new CreateTenantResponse(
            TenantId: tenant.Id,
            Slug: tenant.Slug,
            LegalName: tenant.LegalName,
            Status: tenant.Status,
            CreatedAtUtc: tenant.CreatedAtUtc,
            SandboxEnvironmentId: sandboxEnvironment.Id,
            ProductionEnvironmentId: productionEnvironment.Id
        ));
    }

    private static TenantSettings CreateDefaultTenantSettings(Guid tenantId, TenantEnvironment environment, string legalName)
    {
        return new TenantSettings
        {
            TenantId = tenantId,
            Environment = environment,
            LegalName = legalName,
            FeatureBillingEnabled = false,
            FeatureDocuSignEnabled = false,
            FeatureInStoreSignaturesEnabled = false,
            FeatureFutureAIEnabled = false,
            FeatureDmvSubmissionEnabled = false
        };
    }
}
