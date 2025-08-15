using AiAgent.Api.Domain.Chat.Enums;

namespace AiAgent.Api.Domain.AI.Interfaces;

public interface IAiService
{
    AiProvider Provider { get; }
    Task<string> GetChatCompletionAsync(string userMessage, string instructions);
}
