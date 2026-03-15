using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using CoreConnect.Server.Data;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;
using Xunit;

namespace CoreConnect.Tests.IntegrationTests;

/// <summary>
/// Shared fixture that spins up a SQL Server container for integration tests.
/// Implements IAsyncLifetime so xUnit manages its lifecycle.
/// </summary>
public class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public AppDb CreateDbContext()
    {
        var context = new SqlServerDbContext(BuildConfig(), BuildHostEnv());
        return context;
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        // Ensure schema is created.
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:SQLServer"] = ConnectionString
            })
            .Build();
    }

    private static Microsoft.AspNetCore.Hosting.IWebHostEnvironment BuildHostEnv()
    {
        return new TestHostEnvironment();
    }
}

/// <summary>
/// Shared fixture that spins up a PostgreSQL container for integration tests.
/// </summary>
public class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public AppDb CreateDbContext()
    {
        var context = new PostgreSqlDbContext(BuildConfig(), BuildHostEnv());
        return context;
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = ConnectionString
            })
            .Build();
    }

    private static Microsoft.AspNetCore.Hosting.IWebHostEnvironment BuildHostEnv()
    {
        return new TestHostEnvironment();
    }
}

/// <summary>
/// Shared fixture for SQLite integration tests. No container needed —
/// uses a temporary file-based database.
/// </summary>
public class SqliteFixture : IAsyncLifetime
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"coreconnect_test_{Guid.NewGuid():N}.db");

    public string ConnectionString => $"Data Source={_dbPath}";

    public AppDb CreateDbContext()
    {
        var context = new SqliteDbContext(BuildConfig(), BuildHostEnv());
        return context;
    }

    public async Task InitializeAsync()
    {
        await using var db = CreateDbContext();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    private IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:SQLite"] = ConnectionString
            })
            .Build();
    }

    private static Microsoft.AspNetCore.Hosting.IWebHostEnvironment BuildHostEnv()
    {
        return new TestHostEnvironment();
    }
}

/// <summary>
/// Minimal IWebHostEnvironment implementation for testing.
/// </summary>
internal class TestHostEnvironment : Microsoft.AspNetCore.Hosting.IWebHostEnvironment
{
    public string EnvironmentName { get; set; } = "Testing";
    public string ApplicationName { get; set; } = "CoreConnect.IntegrationTests";
    public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; } = Directory.GetCurrentDirectory();
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
}
