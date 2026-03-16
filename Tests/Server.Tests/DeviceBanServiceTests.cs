using CoreConnect.Server.Models;
using CoreConnect.Server.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreConnect.Server.Tests;

[TestClass]
public class DeviceBanServiceTests
{
    private Mock<IDataService> _dataServiceMock = null!;
    private Mock<ILogger<DeviceBanService>> _loggerMock = null!;
    private DeviceBanService _deviceBanService = null!;
    private SettingsModel _settings = null!;

    [TestInitialize]
    public void TestInit()
    {
        _dataServiceMock = new Mock<IDataService>();
        _loggerMock = new Mock<ILogger<DeviceBanService>>();
        _settings = new SettingsModel();
        _dataServiceMock.Setup(x => x.GetSettings()).ReturnsAsync(_settings);
        _deviceBanService = new DeviceBanService(_dataServiceMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task IsBanned_NoIdentifiers_ReturnsFalse()
    {
        var result = await _deviceBanService.IsBanned();
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task IsBanned_EmptyIdentifiers_ReturnsFalse()
    {
        var result = await _deviceBanService.IsBanned("", "   ");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task IsBanned_NoBannedDevices_ReturnsFalse()
    {
        var result = await _deviceBanService.IsBanned("Device1");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task IsBanned_ExactMatch_ReturnsTrue()
    {
        _settings.BannedDevices = new List<string> { "Device1" };
        var result = await _deviceBanService.IsBanned("Device1");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task IsBanned_CaseInsensitiveMatch_ReturnsTrue()
    {
        _settings.BannedDevices = new List<string> { "Device1" };
        var result = await _deviceBanService.IsBanned("device1");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task IsBanned_TrimmedMatch_ReturnsTrue()
    {
        _settings.BannedDevices = new List<string> { "Device1" };
        var result = await _deviceBanService.IsBanned("  Device1  ");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task IsBanned_MultipleIdentifiers_OneMatches_ReturnsTrue()
    {
        _settings.BannedDevices = new List<string> { "BannedDevice" };
        var result = await _deviceBanService.IsBanned("SafeDevice", "BannedDevice", "127.0.0.1");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task IsBanned_MultipleBannedDevices_ReturnsTrue()
    {
        _settings.BannedDevices = new List<string> { "Banned1", "Banned2" };
        var result = await _deviceBanService.IsBanned("Banned2");
        Assert.IsTrue(result);
    }
}
