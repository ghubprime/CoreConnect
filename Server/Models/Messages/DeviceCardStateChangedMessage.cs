using CoreConnect.Server.Enums;

namespace CoreConnect.Server.Models.Messages;

public record DeviceCardStateChangedMessage(string DeviceId, DeviceCardState State);