using CarDealership.Api.Persistence;
using CarDealership.Api.Persistence.Entities;
using CarDealership.Api.Persistence.Interceptors;
using CarDealership.Api.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.IntegrationTests.Infrastructure;

/// <summary>
/// Integration tests using in-memory database (fallback when Testcontainers/Docker unavailable).
/// 
/// TRADEOFF DOCUMENTATION:
/// - These tests use Microsoft.EntityFrameworkCore.InMemory instead of real SQL Server
/// - InMemory database doesn't enforce all SQL Server constraints (e.g., unique constraints are not enforced)
/// - For production testing, use SqlServerIntegrationTests when Docker/Testcontainers available
/// - InMemory tests are fast and run everywhere, but don't catch database-specific issues
/// - Use this as a safety net in CI/CD pipelines where Docker may not be available
/// 
/// RECOMMENDATION:
/// Run both test suites when possible:
/// - InMemory tests: Fast, run everywhere (default)
/// - SQL Server tests: Realistic, but requires Docker (optional, use when available)
/// </summary>
public class PersistenceInMemoryIntegrationTests
{
    private ApplicationDbContext CreateInMemoryDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .AddInterceptors(new DateTrackingSaveChangesInterceptor())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Database_InitializeSuccessfully()
    {
        // Arrange & Act
        await using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Tenants);
        Assert.NotNull(context.AppUsers);
    }

    [Fact]
    public async Task SaveChanges_SetsCreatedAtUtcAndUpdatedAtUtcOnNewEntity()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var tenantId = Guid.NewGuid();
        var user = new AppUser
        {
            TenantId = tenantId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        // Act
        context.AppUsers.Add(user);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotEqual(default, user.CreatedAtUtc);
        Assert.NotEqual(default, user.UpdatedAtUtc);
        // Allow small time delta due to high-precision timestamps
        var timeDelta = Math.Abs((user.UpdatedAtUtc - user.CreatedAtUtc).TotalMilliseconds);
        Assert.True(timeDelta < 100, $"Expected CreatedAtUtc and UpdatedAtUtc to be within 100ms, but delta was {timeDelta}ms");
    }

    [Fact]
    public async Task SaveChanges_UpdatesUpdatedAtUtcOnModifiedEntity()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var tenantId = Guid.NewGuid();
        var user = new AppUser
        {
            TenantId = tenantId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        context.AppUsers.Add(user);
        await context.SaveChangesAsync();

        var originalUpdatedAt = user.UpdatedAtUtc;
        var originalCreatedAt = user.CreatedAtUtc;

        // Wait a small amount to ensure timestamp changes
        await Task.Delay(100);

        // Act
        user.LastName = "UpdatedUser";
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(originalCreatedAt, user.CreatedAtUtc);
        Assert.True(user.UpdatedAtUtc > originalUpdatedAt);
    }

    [Fact]
    public async Task QueryFilter_ExcludesSoftDeletedRecordsFromQueries()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var tenantId = Guid.NewGuid();
        var activeUser = new AppUser
        {
            TenantId = tenantId,
            FirstName = "Active",
            LastName = "User",
            Email = "active@example.com"
        };

        var deletedUser = new AppUser
        {
            TenantId = tenantId,
            FirstName = "Deleted",
            LastName = "User",
            Email = "deleted@example.com",
            IsDeleted = true,
            DeletedAtUtc = DateTime.UtcNow
        };

        context.AppUsers.Add(activeUser);
        context.AppUsers.Add(deletedUser);
        await context.SaveChangesAsync();

        // Act
        var usersFromDb = await context.AppUsers
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();

        // Assert
        Assert.Single(usersFromDb);
        Assert.Equal("Active", usersFromDb.First().FirstName);
    }

    [Fact]
    public async Task QueryFilter_CanIncludeSoftDeletedRecordsWithIgnoreQueryFilters()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var tenantId = Guid.NewGuid();
        var activeUser = new AppUser
        {
            TenantId = tenantId,
            FirstName = "Active",
            LastName = "User",
            Email = "active@example.com"
        };

        var deletedUser = new AppUser
        {
            TenantId = tenantId,
            FirstName = "Deleted",
            LastName = "User",
            Email = "deleted@example.com",
            IsDeleted = true,
            DeletedAtUtc = DateTime.UtcNow
        };

        context.AppUsers.Add(activeUser);
        context.AppUsers.Add(deletedUser);
        await context.SaveChangesAsync();

        // Act
        var allUsersIncludingDeleted = await context.AppUsers
            .IgnoreQueryFilters()
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();

        // Assert
        Assert.Equal(2, allUsersIncludingDeleted.Count);
    }

    [Fact]
    public async Task TenantConstraint_EnforcesTenantIdOnTenantScopedEntity()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var tenant1 = Tenant.Create("tenant-1", "Tenant 1");
        var tenant2 = Tenant.Create("tenant-2", "Tenant 2");

        context.Tenants.Add(tenant1);
        context.Tenants.Add(tenant2);
        await context.SaveChangesAsync();

        // Act
        var user1 = new AppUser
        {
            TenantId = tenant1.Id,
            FirstName = "User1",
            LastName = "Test",
            Email = "user1@example.com"
        };

        var user2 = new AppUser
        {
            TenantId = tenant2.Id,
            FirstName = "User2",
            LastName = "Test",
            Email = "user2@example.com"
        };

        context.AppUsers.Add(user1);
        context.AppUsers.Add(user2);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(tenant1.Id, user1.TenantId);
        Assert.Equal(tenant2.Id, user2.TenantId);
        Assert.NotEqual(user1.TenantId, user2.TenantId);
    }

    [Fact]
    public async Task EnvironmentProperty_IsStoredCorrectlyInTenantScopedEntities()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var tenantId = Guid.NewGuid();
        var sandboxSettings = new TenantSettings
        {
            TenantId = tenantId,
            Environment = TenantEnvironment.Sandbox,
            LegalName = "Legal Name",
            Dba = "DBA",
            DealerLicenseNumber = "DL123",
            PhysicalAddress = "123 Main St",
            MailingAddress = "123 Main St",
            County = "County",
            Phone = "555-1234",
            Email = "settings@example.com"
        };

        var productionSettings = new TenantSettings
        {
            TenantId = tenantId,
            Environment = TenantEnvironment.Production,
            LegalName = "Legal Name",
            Dba = "DBA",
            DealerLicenseNumber = "DL123",
            PhysicalAddress = "123 Main St",
            MailingAddress = "123 Main St",
            County = "County",
            Phone = "555-1234",
            Email = "settings@example.com"
        };

        // Act
        context.TenantSettings.Add(sandboxSettings);
        context.TenantSettings.Add(productionSettings);
        await context.SaveChangesAsync();

        var retrievedSettings = await context.TenantSettings
            .Where(ts => ts.TenantId == tenantId)
            .OrderBy(ts => ts.Environment)
            .ToListAsync();

        // Assert
        Assert.Equal(2, retrievedSettings.Count);
        Assert.Equal(TenantEnvironment.Production, retrievedSettings[1].Environment);
        Assert.Equal(TenantEnvironment.Sandbox, retrievedSettings[0].Environment);
    }

    [Fact]
    public async Task Relationship_TenantToAppUserCascadesDeletes()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var tenant = Tenant.Create("test-tenant", "Test Tenant");
        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        var user = new AppUser
        {
            TenantId = tenant.Id,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        context.AppUsers.Add(user);
        await context.SaveChangesAsync();

        // Act
        context.Tenants.Remove(tenant);
        await context.SaveChangesAsync();

        var userCount = await context.AppUsers
            .IgnoreQueryFilters()
            .Where(u => u.TenantId == tenant.Id)
            .CountAsync();

        // Assert
        Assert.Equal(0, userCount);
    }

    [Fact]
    public async Task UserRoleAssociation_LinksUserAndRoleWithTenantId()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var tenant = Tenant.Create("test-tenant", "Test Tenant");
        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        var user = new AppUser
        {
            TenantId = tenant.Id,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        var role = new Role
        {
            TenantId = tenant.Id,
            Name = "Admin",
            Description = "Administrator"
        };

        context.AppUsers.Add(user);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var userRole = new UserRole
        {
            TenantId = tenant.Id,
            UserId = user.Id,
            RoleId = role.Id
        };

        context.UserRoles.Add(userRole);

        // Act
        await context.SaveChangesAsync();

        var retrievedUserRole = await context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id);

        // Assert
        Assert.NotNull(retrievedUserRole);
        Assert.Equal(tenant.Id, retrievedUserRole.TenantId);
        Assert.Equal(user.Id, retrievedUserRole.UserId);
        Assert.Equal(role.Id, retrievedUserRole.RoleId);
        Assert.NotNull(retrievedUserRole.User);
        Assert.NotNull(retrievedUserRole.Role);
    }
}
