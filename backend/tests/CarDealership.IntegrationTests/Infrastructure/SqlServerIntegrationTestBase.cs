using CarDealership.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace CarDealership.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for SQL Server integration tests using Testcontainers.
/// Manages the lifecycle of a SQL Server container and provides a configured DbContext.
/// </summary>
public abstract class SqlServerIntegrationTestBase : IAsyncLifetime
{
    private MsSqlContainer? _container;
    protected ApplicationDbContext? DbContext;

    /// <summary>
    /// Gets the connection string for the running SQL Server instance.
    /// </summary>
    protected string ConnectionString => _container?.GetConnectionString() ?? throw new InvalidOperationException("Container not initialized");

    /// <summary>
    /// Gets the running SQL Server container.
    /// </summary>
    protected MsSqlContainer Container => _container ?? throw new InvalidOperationException("Container not initialized");

    /// <summary>
    /// Initializes the SQL Server container and creates the database schema.
    /// </summary>
    public async Task InitializeAsync()
    {
        _container = new MsSqlBuilder()
            .WithPassword("P@ssw0rd123!")
            .Build();

        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        DbContext = new ApplicationDbContext(options);
        await DbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Stops the SQL Server container and cleans up resources.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (DbContext != null)
        {
            await DbContext.DisposeAsync();
        }

        if (_container != null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }

    /// <summary>
    /// Resets the database by dropping and recreating it.
    /// Useful for test isolation.
    /// </summary>
    protected async Task ResetDatabaseAsync()
    {
        if (DbContext != null)
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.Database.MigrateAsync();
        }
    }
}
