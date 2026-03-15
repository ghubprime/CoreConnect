using CoreConnect.Server.Data;
using CoreConnect.Shared.Entities;
using CoreConnect.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoreConnect.Tests.IntegrationTests;

// ──────────────────────────────────────────────────────────────────
//  SQL Server
// ──────────────────────────────────────────────────────────────────

[Collection("SqlServer")]
public class SqlServerAlertRuleTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;
    public SqlServerAlertRuleTests(SqlServerFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CanCreateAndQueryAlertRule()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "AlertOrg" };
        db.Organizations.Add(org);

        var rule = new AlertRule
        {
            OrganizationID = org.ID,
            Name = "High CPU Alert",
            Metric = "CpuUtilization",
            Operator = ">",
            Threshold = 0.90,
            IsEnabled = true
        };
        db.AlertRules.Add(rule);
        await db.SaveChangesAsync();

        var loaded = await db.AlertRules.FirstOrDefaultAsync(r => r.ID == rule.ID);
        Assert.NotNull(loaded);
        Assert.Equal("High CPU Alert", loaded!.Name);
        Assert.Equal(0.90, loaded.Threshold);
    }

    [Fact]
    public async Task CanDeleteAlertRule()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "DeleteAlertOrg" };
        db.Organizations.Add(org);

        var rule = new AlertRule
        {
            OrganizationID = org.ID,
            Name = "Temp Rule",
            Metric = "UsedMemoryPercent",
            Operator = ">",
            Threshold = 0.80
        };
        db.AlertRules.Add(rule);
        await db.SaveChangesAsync();

        db.AlertRules.Remove(rule);
        await db.SaveChangesAsync();

        var deleted = await db.AlertRules.FindAsync(rule.ID);
        Assert.Null(deleted);
    }
}

[Collection("SqlServer")]
public class SqlServerApiTokenPermissionTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;
    public SqlServerApiTokenPermissionTests(SqlServerFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task TokenPermission_DefaultsToAll()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "TokenOrg" };
        db.Organizations.Add(org);

        var token = new ApiToken
        {
            OrganizationID = org.ID,
            Name = "Full Access Token",
            Secret = "hash-placeholder"
        };
        db.ApiTokens.Add(token);
        await db.SaveChangesAsync();

        var loaded = await db.ApiTokens.FindAsync(token.ID);
        Assert.NotNull(loaded);
        Assert.Equal(ApiPermission.All, loaded!.Permissions);
    }

    [Fact]
    public async Task TokenPermission_RoundTrips_ScopedPermissions()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "ScopedOrg" };
        db.Organizations.Add(org);

        var scoped = ApiPermission.DeviceRead | ApiPermission.AlertRead;

        var token = new ApiToken
        {
            OrganizationID = org.ID,
            Name = "Read-Only Token",
            Secret = "hash-placeholder",
            Permissions = scoped
        };
        db.ApiTokens.Add(token);
        await db.SaveChangesAsync();

        var loaded = await db.ApiTokens.FindAsync(token.ID);
        Assert.NotNull(loaded);
        Assert.Equal(scoped, loaded!.Permissions);
        Assert.True(loaded.Permissions.HasFlag(ApiPermission.DeviceRead));
        Assert.True(loaded.Permissions.HasFlag(ApiPermission.AlertRead));
        Assert.False(loaded.Permissions.HasFlag(ApiPermission.ScriptExecute));
    }
}

[Collection("SqlServer")]
public class SqlServerDeviceMacAddressTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;
    public SqlServerDeviceMacAddressTests(SqlServerFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task MacAddresses_RoundTrip()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "MacOrg" };
        db.Organizations.Add(org);

        var macs = new[] { "AA:BB:CC:DD:EE:FF", "11:22:33:44:55:66" };
        var device = new Device
        {
            ID = Guid.NewGuid().ToString(),
            OrganizationID = org.ID,
            MacAddresses = macs
        };
        db.Devices.Add(device);
        await db.SaveChangesAsync();

        var loaded = await db.Devices.FindAsync(device.ID);
        Assert.NotNull(loaded);
        Assert.NotNull(loaded!.MacAddresses);
        Assert.Equal(2, loaded.MacAddresses.Length);
        Assert.Contains("AA:BB:CC:DD:EE:FF", loaded.MacAddresses);
        Assert.Contains("11:22:33:44:55:66", loaded.MacAddresses);
    }
}

// ──────────────────────────────────────────────────────────────────
//  PostgreSQL
// ──────────────────────────────────────────────────────────────────

[Collection("PostgreSql")]
public class PgAlertRuleTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;
    public PgAlertRuleTests(PostgreSqlFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CanCreateAndQueryAlertRule()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "PG AlertOrg" };
        db.Organizations.Add(org);

        var rule = new AlertRule
        {
            OrganizationID = org.ID,
            Name = "PG High CPU Alert",
            Metric = "CpuUtilization",
            Operator = ">",
            Threshold = 0.85,
            IsEnabled = true
        };
        db.AlertRules.Add(rule);
        await db.SaveChangesAsync();

        var loaded = await db.AlertRules.FirstOrDefaultAsync(r => r.ID == rule.ID);
        Assert.NotNull(loaded);
        Assert.Equal("PG High CPU Alert", loaded!.Name);
    }
}

[Collection("PostgreSql")]
public class PgApiTokenPermissionTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;
    public PgApiTokenPermissionTests(PostgreSqlFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task TokenPermission_DefaultsToAll()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "PG TokenOrg" };
        db.Organizations.Add(org);

        var token = new ApiToken
        {
            OrganizationID = org.ID,
            Name = "PG Full Token",
            Secret = "hash-placeholder"
        };
        db.ApiTokens.Add(token);
        await db.SaveChangesAsync();

        var loaded = await db.ApiTokens.FindAsync(token.ID);
        Assert.NotNull(loaded);
        Assert.Equal(ApiPermission.All, loaded!.Permissions);
    }

    [Fact]
    public async Task TokenPermission_RoundTrips_ScopedPermissions()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "PG ScopedOrg" };
        db.Organizations.Add(org);

        var scoped = ApiPermission.WakeOnLan | ApiPermission.DeviceWrite;

        var token = new ApiToken
        {
            OrganizationID = org.ID,
            Name = "PG WoL+Write Token",
            Secret = "hash-placeholder",
            Permissions = scoped
        };
        db.ApiTokens.Add(token);
        await db.SaveChangesAsync();

        var loaded = await db.ApiTokens.FindAsync(token.ID);
        Assert.NotNull(loaded);
        Assert.Equal(scoped, loaded!.Permissions);
        Assert.True(loaded.Permissions.HasFlag(ApiPermission.WakeOnLan));
        Assert.False(loaded.Permissions.HasFlag(ApiPermission.ServerAdmin));
    }
}

[Collection("PostgreSql")]
public class PgDeviceMacAddressTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;
    public PgDeviceMacAddressTests(PostgreSqlFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task MacAddresses_RoundTrip()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "PG MacOrg" };
        db.Organizations.Add(org);

        var macs = new[] { "FF:EE:DD:CC:BB:AA", "66:55:44:33:22:11" };
        var device = new Device
        {
            ID = Guid.NewGuid().ToString(),
            OrganizationID = org.ID,
            MacAddresses = macs
        };
        db.Devices.Add(device);
        await db.SaveChangesAsync();

        var loaded = await db.Devices.FindAsync(device.ID);
        Assert.NotNull(loaded);
        Assert.NotNull(loaded!.MacAddresses);
        Assert.Equal(2, loaded.MacAddresses.Length);
        Assert.Contains("FF:EE:DD:CC:BB:AA", loaded.MacAddresses);
    }
}
