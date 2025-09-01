using AiAgent.Api.Domain.Chat.Enums;

namespace AiAgent.Api.Domain.AI.Interfaces;

public interface IAiProvider
{
    Task<string> GetChatCompletionAsync(string userMessage, string instructions);
    AiProvider ProviderType { get; }
}
