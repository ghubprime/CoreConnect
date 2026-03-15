using CoreConnect.Shared.Enums;

namespace CoreConnect.Desktop.Shared.Abstractions;

public interface IRemoteControlAccessService
{
    bool IsPromptOpen { get; }

    Task<PromptForAccessResult> PromptForAccess(string requesterName, string organizationName);
}
