namespace CoreConnect.Shared.Enums;

/// <summary>
/// Fine-grained permission flags for API tokens.
/// Tokens default to <see cref="All"/> for backwards compatibility.
/// </summary>
[Flags]
public enum ApiPermission
{
    None            = 0,
    DeviceRead      = 1 << 0,
    DeviceWrite     = 1 << 1,
    ScriptExecute   = 1 << 2,
    AlertRead       = 1 << 3,
    AlertWrite      = 1 << 4,
    RemoteControl   = 1 << 5,
    ServerAdmin     = 1 << 6,
    WakeOnLan       = 1 << 7,
    All             = ~0
}
