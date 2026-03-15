using System.Threading.Tasks;

namespace CoreConnect.Agent.Interfaces;

public interface IUpdater
{
    Task BeginChecking();
    Task CheckForUpdates();
    Task InstallLatestVersion();
}