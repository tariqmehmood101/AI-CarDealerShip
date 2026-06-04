using CarDealership.Api.Persistence;
using CarDealership.Api.Persistence.Entities;
using CarDealership.Api.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.IntegrationTests.Infrastructure;

/// <summary>
/// Integration tests verifying database migrations, entity persistence, and query filters.
/// </summary>
public class PersistenceIntegrationTests : SqlServerIntegrationTestBase
{
    [Fact]
    public async Task Database_MigrationAppliesToDatabaseSuccessfully()
    {
        // Arrange & Act
        var database = DbContext!.Database;
        var allMigrations = (await database.GetAppliedMigrationsAsync()).ToList();

        // Assert
        Assert.NotEmpty(allMigrations);
        Assert.Contains(allMigrations, m => m.Contains("InitialCreate"));
    }

    [Fact]
    public async Task SaveChanges_SetsCreatedAtUtcAndUpdatedAtUtcOnNewEntity()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var user = new AppUser
        {
            TenantId = tenantId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        // Act
        DbContext!.AppUsers.Add(user);
        await DbContext.SaveChangesAsync();

        // Assert
        Assert.NotEqual(default, user.CreatedAtUtc);
        Assert.NotEqual(default, user.UpdatedAtUtc);
        Assert.Equal(user.CreatedAtUtc, user.UpdatedAtUtc);
    }

    [Fact]
    public async Task SaveChanges_UpdatesUpdatedAtUtcOnModifiedEntity()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var user = new AppUser
        {
            TenantId = tenantId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        DbContext!.AppUsers.Add(user);
        await DbContext.SaveChangesAsync();

        var originalUpdatedAt = user.UpdatedAtUtc;
        var originalCreatedAt = user.CreatedAtUtc;

        // Wait a small amount to ensure timestamp changes
        await Task.Delay(100);

        // Act
        user.LastName = "UpdatedUser";
        await DbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(originalCreatedAt, user.CreatedAtUtc);
        Assert.True(user.UpdatedAtUtc > originalUpdatedAt);
    }

    [Fact]
    public async Task QueryFilter_ExcludesSoftDeletedRecordsFromQueries()
    {
        // Arrange
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

        DbContext!.AppUsers.Add(activeUser);
        DbContext.AppUsers.Add(deletedUser);
        await DbContext.SaveChangesAsync();

        // Act
        var usersFromDb = await DbContext.AppUsers
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

        DbContext!.AppUsers.Add(activeUser);
        DbContext.AppUsers.Add(deletedUser);
        await DbContext.SaveChangesAsync();

        // Act
        var allUsersIncludingDeleted = await DbContext.AppUsers
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
        var tenant1 = Tenant.Create("tenant-1", "Tenant 1");
        var tenant2 = Tenant.Create("tenant-2", "Tenant 2");

        DbContext!.Tenants.Add(tenant1);
        DbContext.Tenants.Add(tenant2);
        await DbContext.SaveChangesAsync();

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

        DbContext.AppUsers.Add(user1);
        DbContext.AppUsers.Add(user2);
        await DbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(tenant1.Id, user1.TenantId);
        Assert.Equal(tenant2.Id, user2.TenantId);
        Assert.NotEqual(user1.TenantId, user2.TenantId);
    }

    [Fact]
    public async Task EnvironmentProperty_IsStoredCorrectlyInTenantScopedEntities()
    {
        // Arrange
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
        DbContext!.TenantSettings.Add(sandboxSettings);
        DbContext.TenantSettings.Add(productionSettings);
        await DbContext.SaveChangesAsync();

        var retrievedSettings = await DbContext.TenantSettings
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
        var tenant = Tenant.Create("test-tenant", "Test Tenant");
        DbContext!.Tenants.Add(tenant);
        await DbContext.SaveChangesAsync();

        var user = new AppUser
        {
            TenantId = tenant.Id,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        DbContext.AppUsers.Add(user);
        await DbContext.SaveChangesAsync();

        // Act
        DbContext.Tenants.Remove(tenant);
        await DbContext.SaveChangesAsync();

        var userCount = await DbContext.AppUsers
            .IgnoreQueryFilters()
            .Where(u => u.TenantId == tenant.Id)
            .CountAsync();

        // Assert
        Assert.Equal(0, userCount);
    }

    [Fact]
    public async Task UniqueConstraint_EnforcesTenantSlugUniqueness()
    {
        // Arrange
        var tenant1 = Tenant.Create("unique-slug", "Tenant 1");
        var tenant2 = Tenant.Create("unique-slug", "Tenant 2");

        DbContext!.Tenants.Add(tenant1);
        await DbContext.SaveChangesAsync();

        DbContext.Tenants.Add(tenant2);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task UserRoleAssociation_LinksUserAndRoleWithTenantId()
    {
        // Arrange
        var tenant = Tenant.Create("test-tenant", "Test Tenant");
        DbContext!.Tenants.Add(tenant);
        await DbContext.SaveChangesAsync();

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

        DbContext.AppUsers.Add(user);
        DbContext.Roles.Add(role);
        await DbContext.SaveChangesAsync();

        var userRole = new UserRole
        {
            TenantId = tenant.Id,
            UserId = user.Id,
            RoleId = role.Id
        };

        DbContext.UserRoles.Add(userRole);

        // Act
        await DbContext.SaveChangesAsync();

        var retrievedUserRole = await DbContext.UserRoles
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
