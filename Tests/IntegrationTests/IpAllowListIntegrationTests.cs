using CoreConnect.Server.Data;
using CoreConnect.Server.Middleware;
using CoreConnect.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace CoreConnect.Tests.IntegrationTests;

// ──────────────────────────────────────────────────────────────────
//  SQL Server — IP Allow-List
// ──────────────────────────────────────────────────────────────────

[Collection("SqlServer")]
public class SqlServerIpAllowListTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;
    public SqlServerIpAllowListTests(SqlServerFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AllowedIpRanges_RoundTrips()
    {
        await using var db = _fixture.CreateDbContext();

        var ranges = new[] { "10.0.0.0/8", "192.168.1.100" };
        var org = new Organization
        {
            ID = Guid.NewGuid().ToString(),
            OrganizationName = "IP Org",
            AllowedIpRanges = ranges
        };
        db.Organizations.Add(org);
        await db.SaveChangesAsync();

        var loaded = await db.Organizations.FindAsync(org.ID);
        Assert.NotNull(loaded);
        Assert.NotNull(loaded!.AllowedIpRanges);
        Assert.Equal(2, loaded.AllowedIpRanges.Length);
        Assert.Contains("10.0.0.0/8", loaded.AllowedIpRanges);
        Assert.Contains("192.168.1.100", loaded.AllowedIpRanges);
    }

    [Fact]
    public async Task AllowedIpRanges_NullByDefault()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization
        {
            ID = Guid.NewGuid().ToString(),
            OrganizationName = "NoIP Org"
        };
        db.Organizations.Add(org);
        await db.SaveChangesAsync();

        var loaded = await db.Organizations.FindAsync(org.ID);
        Assert.NotNull(loaded);
        Assert.Null(loaded!.AllowedIpRanges);
    }
}

// ──────────────────────────────────────────────────────────────────
//  PostgreSQL — IP Allow-List
// ──────────────────────────────────────────────────────────────────

[Collection("PostgreSql")]
public class PgIpAllowListTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;
    public PgIpAllowListTests(PostgreSqlFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AllowedIpRanges_RoundTrips()
    {
        await using var db = _fixture.CreateDbContext();

        var ranges = new[] { "172.16.0.0/12", "10.10.10.1" };
        var org = new Organization
        {
            ID = Guid.NewGuid().ToString(),
            OrganizationName = "PG IP Org",
            AllowedIpRanges = ranges
        };
        db.Organizations.Add(org);
        await db.SaveChangesAsync();

        var loaded = await db.Organizations.FindAsync(org.ID);
        Assert.NotNull(loaded);
        Assert.NotNull(loaded!.AllowedIpRanges);
        Assert.Equal(2, loaded.AllowedIpRanges.Length);
        Assert.Contains("172.16.0.0/12", loaded.AllowedIpRanges);
    }
}

// ──────────────────────────────────────────────────────────────────
//  SQLite — IP Allow-List
// ──────────────────────────────────────────────────────────────────

[Collection("Sqlite")]
public class SqliteIpAllowListTests : IClassFixture<SqliteFixture>
{
    private readonly SqliteFixture _fixture;
    public SqliteIpAllowListTests(SqliteFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AllowedIpRanges_RoundTrips()
    {
        await using var db = _fixture.CreateDbContext();

        var ranges = new[] { "192.168.0.0/16" };
        var org = new Organization
        {
            ID = Guid.NewGuid().ToString(),
            OrganizationName = "Lite IP Org",
            AllowedIpRanges = ranges
        };
        db.Organizations.Add(org);
        await db.SaveChangesAsync();

        var loaded = await db.Organizations.FindAsync(org.ID);
        Assert.NotNull(loaded);
        Assert.NotNull(loaded!.AllowedIpRanges);
        Assert.Single(loaded.AllowedIpRanges);
        Assert.Contains("192.168.0.0/16", loaded.AllowedIpRanges);
    }
}

// ──────────────────────────────────────────────────────────────────
//  IP Allow-List Middleware — Unit Tests (no DB needed)
// ──────────────────────────────────────────────────────────────────

public class IpAllowListMiddlewareTests
{
    [Theory]
    [InlineData("10.0.0.5", new[] { "10.0.0.0/8" }, true)]
    [InlineData("10.0.0.5", new[] { "192.168.0.0/16" }, false)]
    [InlineData("192.168.1.100", new[] { "192.168.1.100" }, true)]
    [InlineData("192.168.1.101", new[] { "192.168.1.100" }, false)]
    [InlineData("172.16.5.10", new[] { "172.16.0.0/12", "10.0.0.0/8" }, true)]
    [InlineData("8.8.8.8", new[] { "10.0.0.0/8", "172.16.0.0/12" }, false)]
    public void IsIpAllowed_ReturnsExpected(string ipStr, string[] ranges, bool expected)
    {
        var ip = IPAddress.Parse(ipStr);
        var result = IpAllowListMiddleware.IsIpAllowed(ip, ranges);
        Assert.Equal(expected, result);
    }
}
