using CoreConnect.Server.Data;
using CoreConnect.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoreConnect.Tests.IntegrationTests;

/// <summary>
/// Integration tests that exercise DataService CRUD operations against real database containers.
/// These tests require Docker to be running.
/// </summary>
[Collection("SqlServer")]
public class SqlServerDataServiceTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public SqlServerDataServiceTests(SqlServerFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CanCreateAndQueryOrganization()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "Test Org" };
        db.Organizations.Add(org);
        await db.SaveChangesAsync();

        var loaded = await db.Organizations.FindAsync(org.ID);
        Assert.NotNull(loaded);
        Assert.Equal("Test Org", loaded!.OrganizationName);
    }

    [Fact]
    public async Task CanAddDeviceAndTelemetrySnapshot()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "Telemetry Org" };
        db.Organizations.Add(org);

        var device = new Device { ID = Guid.NewGuid().ToString(), OrganizationID = org.ID };
        db.Devices.Add(device);

        var snapshot = new DeviceTelemetrySnapshot
        {
            DeviceId = device.ID,
            CpuUtilization = 0.42,
            UsedMemoryPercent = 0.65,
            UsedStoragePercent = 0.30,
            Timestamp = DateTimeOffset.UtcNow
        };
        db.TelemetrySnapshots.Add(snapshot);
        await db.SaveChangesAsync();

        var snapshots = await db.TelemetrySnapshots
            .Where(s => s.DeviceId == device.ID)
            .ToListAsync();

        Assert.Single(snapshots);
        Assert.Equal(0.42, snapshots[0].CpuUtilization);
    }
}

[Collection("PostgreSql")]
public class PostgreSqlDataServiceTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlDataServiceTests(PostgreSqlFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CanCreateAndQueryOrganization()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "PG Test Org" };
        db.Organizations.Add(org);
        await db.SaveChangesAsync();

        var loaded = await db.Organizations.FindAsync(org.ID);
        Assert.NotNull(loaded);
        Assert.Equal("PG Test Org", loaded!.OrganizationName);
    }

    [Fact]
    public async Task CanAddDeviceAndTelemetrySnapshot()
    {
        await using var db = _fixture.CreateDbContext();

        var org = new Organization { ID = Guid.NewGuid().ToString(), OrganizationName = "PG Telemetry Org" };
        db.Organizations.Add(org);

        var device = new Device { ID = Guid.NewGuid().ToString(), OrganizationID = org.ID };
        db.Devices.Add(device);

        var snapshot = new DeviceTelemetrySnapshot
        {
            DeviceId = device.ID,
            CpuUtilization = 0.55,
            UsedMemoryPercent = 0.70,
            UsedStoragePercent = 0.40,
            Timestamp = DateTimeOffset.UtcNow
        };
        db.TelemetrySnapshots.Add(snapshot);
        await db.SaveChangesAsync();

        var snapshots = await db.TelemetrySnapshots
            .Where(s => s.DeviceId == device.ID)
            .ToListAsync();

        Assert.Single(snapshots);
        Assert.Equal(0.55, snapshots[0].CpuUtilization);
    }
}
