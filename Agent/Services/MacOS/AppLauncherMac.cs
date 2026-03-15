using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using CoreConnect.Agent.Interfaces;

namespace CoreConnect.Agent.Services.MacOS;

public class AppLauncherMac : IAppLauncher
{

    public async Task<int> LaunchChatService(string pipeName, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection)
    {
        await hubConnection.SendAsync("DisplayMessage", "Feature under development.", "Currently unsupported", "bg-warning", userConnectionId);
        return 0;
    }

    public Task LaunchRemoteControl(int targetSessionId, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, bool enableWindowsGpuAcceleration, HubConnection hubConnection)
    {
        return hubConnection.SendAsync("DisplayMessage", "Feature under development.", "Currently unsupported", "bg-warning", userConnectionId);
    }

    public Task RestartScreenCaster(string[] viewerIds, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, bool enableWindowsGpuAcceleration, HubConnection hubConnection, int targetSessionID = -1)
    {
        return hubConnection.SendAsync("DisplayMessage", "Feature under development.", "Currently unsupported", "bg-warning", userConnectionId);
    }
}
