using CoreConnect.Shared.Enums;
using CoreConnect.Shared.Models;

namespace CoreConnect.Server.Models.Messages;

public record PowerShellCompletionsMessage(PwshCommandCompletion Completion, CompletionIntent Intent);