using CoreConnect.Shared.Models;

namespace CoreConnect.Desktop.Shared.Abstractions;

public interface IChatUiService
{
    event EventHandler ChatWindowClosed;

    void ShowChatWindow(string organizationName, StreamWriter writer);
    Task ReceiveChat(ChatMessage chatMessage);
}
