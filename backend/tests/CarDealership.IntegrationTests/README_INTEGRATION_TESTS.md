# EF Core Persistence Integration Tests

## Overview

This directory contains integration tests for the EF Core persistence layer. There are two test suites with different approaches:

## Test Suites

### 1. In-Memory Integration Tests (Primary/Fallback)
**File:** `PersistenceInMemoryIntegrationTests.cs`

- **Used By:** All environments (CI/CD, local development)
- **Database:** Microsoft.EntityFrameworkCore.InMemory
- **Speed:** Very fast (~100-500ms per test)
- **Coverage:** Good for entity relationships, query filters, interceptors
- **Limitations:** Does not validate SQL Server-specific constraints (e.g., unique constraints not enforced in-memory)

**Tradeoff:** In-memory database is a practical fallback that runs everywhere but doesn't catch database-specific issues.

**When to Use:**
- As the default test suite
- In CI/CD pipelines where Docker may not be available
- For rapid local development feedback

### 2. SQL Server Testcontainers Integration Tests (Optional)
**File:** `PersistenceIntegrationTests.cs`  
**Base Class:** `Infrastructure/SqlServerIntegrationTestBase.cs`

- **Used By:** Environments with Docker/Testcontainers support
- **Database:** Real SQL Server in a container
- **Speed:** Slower (~5-30s per test) due to container startup
- **Coverage:** Comprehensive - tests all SQL Server constraints, migrations, indexes
- **Requirements:** Docker Desktop or Docker daemon running

**Prerequisites to Run:**
```bash
# Ensure Docker is running
docker --version

# Run the tests
dotnet test tests/CarDealership.IntegrationTests/CarDealership.IntegrationTests.csproj --logger "console;verbosity=minimal"
```

## Running the Tests

### All Tests (In-Memory Only)
```bash
dotnet test
```

### Specific Test Suite
```bash
# In-memory only
dotnet test --filter "NamespaceName~InMemory"

# SQL Server only (requires Docker)
dotnet test --filter "NamespaceName~PersistenceIntegrationTests" 
```

### With Coverage Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

## Key Test Scenarios Covered

1. **Migration Application**
   - Migrations apply cleanly to a fresh database
   - Schema matches entity configurations

2. **Timestamp Tracking**
   - `CreatedAtUtc` is set on entity creation
   - `UpdatedAtUtc` is updated on modifications
   - `CreatedAtUtc` never changes after creation

3. **Soft-Delete Query Filters**
   - Soft-deleted records are excluded by default
   - Can be included with `IgnoreQueryFilters()`
   - `DeletedAtUtc` is populated when soft-deleted

4. **Tenant Isolation**
   - All tenant-scoped entities require `TenantId`
   - Cascade deletes work correctly (tenant → dependent entities)
   - Unique constraints enforce tenant isolation

5. **Entity Relationships**
   - Foreign keys are properly configured
   - Navigation properties work correctly
   - Cascade behavior matches configuration

6. **Environment Scoping**
   - `Sandbox` and `Production` environments are distinct
   - Settings/records can be per-environment

## Entity Configurations

All entities use `IEntityTypeConfiguration<T>` for Fluent API configuration:

- **TenantConfiguration** - Root aggregate, cascade deletes to all tenant-scoped entities
- **AppUserConfiguration** - Tenant-scoped, unique constraint on (TenantId, Email)
- **RoleConfiguration** - Tenant-scoped, unique constraint on (TenantId, Name)
- **PermissionConfiguration** - Global, unique Name
- **UserRoleConfiguration** - Join table with unique constraint on (TenantId, UserId, RoleId)
- **TenantSettingsConfiguration** - Per-tenant, per-environment settings
- **TenantEnvironmentRecordConfiguration** - Per-tenant environment tracking

## CI/CD Integration

### Recommended Pipeline Strategy

```yaml
# Run fast in-memory tests on every commit
test-inmemory:
  script:
    - dotnet test tests/CarDealership.IntegrationTests/CarDealership.IntegrationTests.csproj --filter "InMemory"

# Run full SQL Server tests periodically (optional, if Docker available)
test-sqlserver:
  script:
    - dotnet test tests/CarDealership.IntegrationTests/CarDealership.IntegrationTests.csproj --filter "PersistenceIntegrationTests"
  only:
    - # on specific branch or schedule
```

## Troubleshooting

### "Cannot detect the Docker endpoint" Error
This is expected when Docker is not available. The in-memory tests will still run successfully.

**Solution:** Use in-memory tests (default behavior)

### "Port already in use" Error
Multiple SQL Server containers are running.

**Solution:** 
```bash
# Stop all containers
docker stop $(docker ps -q)
```

### Test Hangs or Times Out
SQL Server container startup may be slow on first run.

**Solution:** 
- Increase test timeout in `Directory.Build.props` or test configuration
- Ensure sufficient disk space (containers need ~1.5GB)
- Check Docker resource limits (CPU, memory)

## Future Enhancements

- [ ] Add performance benchmarks for query filters
- [ ] Add concurrent tenant isolation tests
- [ ] Add audit trail / CreatedByUserId / UpdatedByUserId tests
- [ ] Add permission model relationship tests
- [ ] Add migration rollback scenarios

## References

- [Entity Framework Core Testing Documentation](https://learn.microsoft.com/en-us/ef/core/testing/)
- [Testcontainers for .NET](https://dotnet.testcontainers.org/)
- [EF Core Fluent API - Entity Type Configuration](https://learn.microsoft.com/en-us/ef/core/modeling/entity-types#configuring-a-dbset)
- [Query Filters in EF Core](https://learn.microsoft.com/en-us/ef/core/querying/filters)
- [SaveChanges Interceptors](https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/interceptors)
