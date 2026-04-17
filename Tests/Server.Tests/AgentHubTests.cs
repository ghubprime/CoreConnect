using CoreConnect.Server.Hubs;
using CoreConnect.Server.Services;
using Bitbound.SimpleMessenger;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CoreConnect.Shared.Extensions;
using CoreConnect.Shared.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace CoreConnect.Server.Tests;

[TestClass]
public class AgentHubTests
{
    private TestData _testData = null!;
    private IDataService _dataService = null!;

    [TestMethod]
    [DoNotParallelize]
    public async Task DeviceCameOnline_BannedByName()
    {
        var circuitManager = new Mock<ICircuitManager>();
        var circuitConnection = new Mock<ICircuitConnection>();
        circuitManager.Setup(x => x.Connections).Returns(new[] { circuitConnection.Object });
        circuitConnection.Setup(x => x.User).Returns(_testData.Org1Admin1);
        var viewerHub = new Mock<IHubContext<ViewerHub>>();
        var expiringTokenService = new Mock<IExpiringTokenService>();
        var serviceSessionCache = new Mock<IAgentHubSessionCache>();
        var remoteControlSessions = new Mock<IRemoteControlSessionCache>();
        var messenger = new Mock<IMessenger>();
        var alertProcessor = new Mock<IAlertRuleProcessor>();
        var scriptConsoleRelay = new Mock<IScriptConsoleRelay>();
        var deviceBanService = new Mock<IDeviceBanService>();
        var logger = new Mock<ILogger<AgentHub>>();

        deviceBanService
            .Setup(x => x.IsBanned(It.Is<string[]>(s => s.Contains(_testData.Org1Device1.DeviceName))))
            .ReturnsAsync(true);

        var hub = new AgentHub(
            _dataService,
            serviceSessionCache.Object,
            viewerHub.Object,
            circuitManager.Object,
            expiringTokenService.Object,
            remoteControlSessions.Object,
            messenger.Object,
            alertProcessor.Object,
            scriptConsoleRelay.Object,
            deviceBanService.Object,
            logger.Object);

        var hubClients = new Mock<IHubCallerClients<IAgentHubClient>>();
        var caller = new Mock<IAgentHubClient>();
        hubClients.Setup(x => x.Caller).Returns(caller.Object);
        hub.Clients = hubClients.Object;

        var result = await hub.DeviceCameOnline(_testData.Org1Device1.ToDto());
        Assert.IsFalse(result);
        hubClients.Verify(x => x.Caller, Times.Once);
        caller.Verify(x => x.UninstallAgent(), Times.Once);
    }

    [TestMethod]
    [DoNotParallelize]
    public async Task DeviceCameOnline_BannedById()
    {
        var circuitManager = new Mock<ICircuitManager>();
        var circuitConnection = new Mock<ICircuitConnection>();
        circuitManager.Setup(x => x.Connections).Returns(new[] { circuitConnection.Object });
        circuitConnection.Setup(x => x.User).Returns(_testData.Org1Admin1);
        var viewerHub = new Mock<IHubContext<ViewerHub>>();
        var expiringTokenService = new Mock<IExpiringTokenService>();
        var serviceSessionCache = new Mock<IAgentHubSessionCache>();
        var remoteControlSessions = new Mock<IRemoteControlSessionCache>();
        var messenger = new Mock<IMessenger>();
        var alertProcessor = new Mock<IAlertRuleProcessor>();
        var scriptConsoleRelay = new Mock<IScriptConsoleRelay>();
        var deviceBanService = new Mock<IDeviceBanService>();
        var logger = new Mock<ILogger<AgentHub>>();

        deviceBanService
            .Setup(x => x.IsBanned(It.Is<string[]>(s => s.Contains(_testData.Org1Device1.ID))))
            .ReturnsAsync(true);

        var hub = new AgentHub(
            _dataService,
            serviceSessionCache.Object,
            viewerHub.Object,
            circuitManager.Object,
            expiringTokenService.Object,
            remoteControlSessions.Object,
            messenger.Object,
            alertProcessor.Object,
            scriptConsoleRelay.Object,
            deviceBanService.Object,
            logger.Object);

        var hubClients = new Mock<IHubCallerClients<IAgentHubClient>>();
        var caller = new Mock<IAgentHubClient>();
        hubClients.Setup(x => x.Caller).Returns(caller.Object);
        hub.Clients = hubClients.Object;

        var result = await hub.DeviceCameOnline(_testData.Org1Device1.ToDto());
        Assert.IsFalse(result);
        hubClients.Verify(x => x.Caller, Times.Once);
        caller.Verify(x => x.UninstallAgent(), Times.Once);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _testData.ClearData();
    }

    [TestInitialize]
    public async Task TestInit()
    {
        _testData = new TestData();
        await _testData.Init();
        _dataService = IoCActivator.ServiceProvider.GetRequiredService<IDataService>();
    }
}
