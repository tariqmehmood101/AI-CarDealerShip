using CarDealership.Api.Persistence;
using CarDealership.Api.Persistence.Entities;
using CarDealership.Api.Shared.Common;
using CarDealership.Api.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Api.Services;

public static class SeedDataExtensions
{
    public static async Task SeedDevelopmentDataAsync(this ApplicationDbContext dbContext)
    {
        // Only seed if this is development and database is empty
        if (await dbContext.Tenants.AnyAsync())
        {
            return; // Already seeded
        }

        // Create a demo tenant
        var demoTenant = Tenant.Create("demo-dealership", "Demo Dealership Inc");
        demoTenant.IsDeleted = false;
        dbContext.Tenants.Add(demoTenant);
        await dbContext.SaveChangesAsync();

        // Create Sandbox environment
        var sandboxEnvironment = new TenantEnvironmentRecord
        {
            TenantId = demoTenant.Id,
            Environment = TenantEnvironment.Sandbox,
            IsDeleted = false
        };

        // Create Production environment
        var productionEnvironment = new TenantEnvironmentRecord
        {
            TenantId = demoTenant.Id,
            Environment = TenantEnvironment.Production,
            IsDeleted = false
        };

        dbContext.TenantEnvironments.Add(sandboxEnvironment);
        dbContext.TenantEnvironments.Add(productionEnvironment);

        // Create default settings for Sandbox
        var sandboxSettings = new TenantSettings
        {
            TenantId = demoTenant.Id,
            Environment = TenantEnvironment.Sandbox,
            LegalName = "Demo Dealership Inc",
            Dba = "Demo Dealership",
            County = "Hillsborough",
            Phone = "(813) 555-0100",
            Email = "demo@demo-dealership.local",
            Website = "https://demo-dealership.local",
            FeatureBillingEnabled = false,
            FeatureDocuSignEnabled = false,
            FeatureInStoreSignaturesEnabled = false,
            FeatureFutureAIEnabled = false,
            FeatureDmvSubmissionEnabled = false,
            IsDeleted = false
        };

        // Create default settings for Production
        var productionSettings = new TenantSettings
        {
            TenantId = demoTenant.Id,
            Environment = TenantEnvironment.Production,
            LegalName = "Demo Dealership Inc",
            Dba = "Demo Dealership",
            County = "Hillsborough",
            Phone = "(813) 555-0100",
            Email = "demo@demo-dealership.local",
            Website = "https://demo-dealership.local",
            FeatureBillingEnabled = false,
            FeatureDocuSignEnabled = false,
            FeatureInStoreSignaturesEnabled = false,
            FeatureFutureAIEnabled = false,
            FeatureDmvSubmissionEnabled = false,
            IsDeleted = false
        };

        dbContext.TenantSettings.Add(sandboxSettings);
        dbContext.TenantSettings.Add(productionSettings);

        await dbContext.SaveChangesAsync();
    }
}
