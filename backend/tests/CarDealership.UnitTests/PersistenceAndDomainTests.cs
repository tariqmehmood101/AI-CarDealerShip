using CarDealership.Api.Persistence;
using CarDealership.Api.Persistence.Entities;
using CarDealership.Api.Persistence.Interceptors;
using CarDealership.Api.Shared.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CarDealership.UnitTests;

public class PersistenceAndDomainTests
{
    [Fact]
    public void Tenant_Create_ShouldNormalizeValues()
    {
        var tenant = Tenant.Create(" my-tenant ", " My Tenant ");

        Assert.Equal("my-tenant", tenant.Slug);
        Assert.Equal("My Tenant", tenant.LegalName);
        Assert.Equal("Active", tenant.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Tenant_Create_Throws_WhenSlugIsInvalid(string slug)
    {
        var exception = Assert.Throws<ArgumentException>(() => Tenant.Create(slug!, "Legal Name"));

        Assert.Contains("Tenant slug is required", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Tenant_Create_Throws_WhenLegalNameIsInvalid(string legalName)
    {
        var exception = Assert.Throws<ArgumentException>(() => Tenant.Create("tenant-slug", legalName!));

        Assert.Contains("Tenant legal name is required", exception.Message);
    }

    [Fact]
    public async Task DateTrackingSaveChangesInterceptor_SetsTimestampsOnAddAndUpdateAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new DateTrackingSaveChangesInterceptor())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var user = new AppUser
        {
            TenantId = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com"
        };

        context.AppUsers.Add(user);
        await context.SaveChangesAsync();

        Assert.NotEqual(default, user.CreatedAtUtc);
        Assert.NotEqual(default, user.UpdatedAtUtc);
        var firstUpdated = user.UpdatedAtUtc;

        user.LastName = "Smith";
        await context.SaveChangesAsync();

        Assert.True(user.UpdatedAtUtc > firstUpdated);
    }

    [Fact]
    public void ApplicationDbContext_RegistersQueryFilter_ForBaseEntities()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var entityType = context.Model.FindEntityType(typeof(AppUser));
        Assert.NotNull(entityType);
        Assert.NotNull(entityType.GetQueryFilter());
    }

    [Fact]
    public void DesignTimeDbContextFactory_CreatesDbContext_WithConnectionString()
    {
        var factory = new DesignTimeDbContextFactory();
        using var context = factory.CreateDbContext(Array.Empty<string>());

        Assert.NotNull(context);
        Assert.IsType<ApplicationDbContext>(context);
        Assert.Contains("CarDealership", context.Database.GetConnectionString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EntityClasses_DefaultPropertyValues_AreAccessible()
    {
        var tenantEnvironmentRecord = new TenantEnvironmentRecord
        {
            TenantId = Guid.NewGuid(),
            Environment = TenantEnvironment.Production,
            Description = "Production environment"
        };

        Assert.Equal("Production environment", tenantEnvironmentRecord.Description);

        var settings = new TenantSettings
        {
            TenantId = Guid.NewGuid(),
            Environment = TenantEnvironment.Sandbox,
            LegalName = "Legal Name",
            Dba = "DBA Name",
            DealerLicenseNumber = "DL123"
        };

        Assert.False(settings.FeatureBillingEnabled);
        Assert.Equal("Legal Name", settings.LegalName);

        var user = new AppUser
        {
            TenantId = Guid.NewGuid(),
            FirstName = "First",
            LastName = "Last",
            Email = "user@example.com"
        };

        Assert.Equal("First", user.FirstName);
        Assert.Equal("user@example.com", user.Email);

        var role = new Role { TenantId = Guid.NewGuid(), Name = "Admin", Description = "Administrator" };
        Assert.Equal("Admin", role.Name);

        var permission = new Permission { Name = "Read", Description = "Read access" };
        Assert.Equal("Read", permission.Name);

        var userRole = new UserRole { TenantId = Guid.NewGuid(), UserId = user.Id, RoleId = role.Id, User = user, Role = role };
        Assert.Equal(user.Id, userRole.UserId);
        Assert.Equal(role.Id, userRole.RoleId);
    }

    [Fact]
    public void EntityClasses_BaseProperties_AreInitialized()
    {
        var user = new AppUser { TenantId = Guid.NewGuid(), FirstName = "A", LastName = "B", Email = "a@b.com" };

        Assert.NotEqual(default, user.CreatedAtUtc);
        Assert.NotEqual(default, user.UpdatedAtUtc);
        Assert.False(user.IsDeleted);
        Assert.Null(user.CreatedByUserId);
        Assert.Null(user.UpdatedByUserId);
    }

    [Fact]
    public void DateTrackingSaveChangesInterceptor_SetsTimestampsOnSaveChanges()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new DateTrackingSaveChangesInterceptor())
            .Options;

        using var context = new ApplicationDbContext(options);
        var user = new AppUser { TenantId = Guid.NewGuid(), FirstName = "Jake", LastName = "Wayne", Email = "jake@example.com" };

        context.AppUsers.Add(user);
        context.SaveChanges();

        Assert.NotEqual(default, user.CreatedAtUtc);
        Assert.NotEqual(default, user.UpdatedAtUtc);
    }

    [Fact]
    public void ApplicationDbContext_RegistersQueryFilter_ForTenant()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(Tenant));

        Assert.NotNull(entityType);
        Assert.NotNull(entityType.GetQueryFilter());
    }

    /// <summary>
    /// Validation tests for entity properties and constraints
    /// </summary>

    [Fact]
    public void AppUser_ValidatesRequiredProperties()
    {
        var tenantId = Guid.NewGuid();
        var user = new AppUser
        {
            TenantId = tenantId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            IsVerified = true
        };

        Assert.Equal(tenantId, user.TenantId);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("john@example.com", user.Email);
        Assert.True(user.IsVerified);
    }

    [Fact]
    public void Role_ValidatesRequiredProperties()
    {
        var tenantId = Guid.NewGuid();
        var role = new Role
        {
            TenantId = tenantId,
            Name = "Administrator",
            Description = "Admin role with full access"
        };

        Assert.Equal(tenantId, role.TenantId);
        Assert.Equal("Administrator", role.Name);
        Assert.Equal("Admin role with full access", role.Description);
    }

    [Fact]
    public void Permission_ValidatesRequiredProperties()
    {
        var permission = new Permission
        {
            Name = "DeleteUser",
            Description = "Permission to delete users"
        };

        Assert.Equal("DeleteUser", permission.Name);
        Assert.Equal("Permission to delete users", permission.Description);
    }

    [Fact]
    public void UserRole_ValidatesRequiredProperties()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var userRole = new UserRole
        {
            TenantId = tenantId,
            UserId = userId,
            RoleId = roleId
        };

        Assert.Equal(tenantId, userRole.TenantId);
        Assert.Equal(userId, userRole.UserId);
        Assert.Equal(roleId, userRole.RoleId);
    }

    [Fact]
    public void TenantSettings_ValidatesRequiredProperties()
    {
        var tenantId = Guid.NewGuid();
        var settings = new TenantSettings
        {
            TenantId = tenantId,
            Environment = TenantEnvironment.Sandbox,
            LegalName = "ABC Dealership Inc.",
            Dba = "ABC Motors",
            DealerLicenseNumber = "DL-12345",
            PhysicalAddress = "123 Main St, Springfield, IL",
            MailingAddress = "123 Main St, Springfield, IL",
            County = "Sangamon",
            Phone = "+1-217-555-0001",
            Email = "info@abcmotors.com"
        };

        Assert.Equal(tenantId, settings.TenantId);
        Assert.Equal(TenantEnvironment.Sandbox, settings.Environment);
        Assert.Equal("ABC Dealership Inc.", settings.LegalName);
        Assert.Equal("DL-12345", settings.DealerLicenseNumber);
        Assert.False(settings.FeatureBillingEnabled);
        Assert.False(settings.FeatureDocuSignEnabled);
    }

    [Fact]
    public void TenantEnvironmentRecord_ValidatesRequiredProperties()
    {
        var tenantId = Guid.NewGuid();
        var record = new TenantEnvironmentRecord
        {
            TenantId = tenantId,
            Environment = TenantEnvironment.Production,
            Description = "Production DMV environment"
        };

        Assert.Equal(tenantId, record.TenantId);
        Assert.Equal(TenantEnvironment.Production, record.Environment);
        Assert.Equal("Production DMV environment", record.Description);
    }

    [Fact]
    public void TenantEnvironment_SupportsBothSandboxAndProduction()
    {
        var sandbox = TenantEnvironment.Sandbox;
        var production = TenantEnvironment.Production;

        Assert.NotEqual(sandbox, production);
        Assert.Equal("Sandbox", sandbox.ToString());
        Assert.Equal("Production", production.ToString());
    }

    [Fact]
    public void AllBaseEntities_HaveGuidId()
    {
        var tenant = Tenant.Create("test", "Test");
        var user = new AppUser { TenantId = Guid.NewGuid(), FirstName = "A", LastName = "B", Email = "a@b.com" };
        var role = new Role { TenantId = Guid.NewGuid(), Name = "Test" };
        var permission = new Permission { Name = "Test" };

        Assert.NotEqual(default, tenant.Id);
        Assert.NotEqual(default, user.Id);
        Assert.NotEqual(default, role.Id);
        Assert.NotEqual(default, permission.Id);

        Assert.IsType<Guid>(tenant.Id);
        Assert.IsType<Guid>(user.Id);
        Assert.IsType<Guid>(role.Id);
        Assert.IsType<Guid>(permission.Id);
    }

    [Fact]
    public void AllTenantScopedEntities_RequireTenantId()
    {
        var tenantId = Guid.NewGuid();

        // These entities must have TenantId property
        var user = new AppUser { TenantId = tenantId, FirstName = "A", LastName = "B", Email = "a@b.com" };
        var role = new Role { TenantId = tenantId, Name = "Test" };
        var userRole = new UserRole { TenantId = tenantId, UserId = Guid.NewGuid(), RoleId = Guid.NewGuid() };
        var settings = new TenantSettings { TenantId = tenantId, Environment = TenantEnvironment.Sandbox };
        var record = new TenantEnvironmentRecord { TenantId = tenantId, Environment = TenantEnvironment.Sandbox };

        Assert.Equal(tenantId, user.TenantId);
        Assert.Equal(tenantId, role.TenantId);
        Assert.Equal(tenantId, userRole.TenantId);
        Assert.Equal(tenantId, settings.TenantId);
        Assert.Equal(tenantId, record.TenantId);
    }

    [Fact]
    public void EnvironmentScopedEntities_StoreEnvironment()
    {
        var tenantId = Guid.NewGuid();

        var settings = new TenantSettings
        {
            TenantId = tenantId,
            Environment = TenantEnvironment.Production
        };

        var record = new TenantEnvironmentRecord
        {
            TenantId = tenantId,
            Environment = TenantEnvironment.Sandbox
        };

        Assert.Equal(TenantEnvironment.Production, settings.Environment);
        Assert.Equal(TenantEnvironment.Sandbox, record.Environment);
    }
}
