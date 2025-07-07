using AiAgent.Api.Domain.Chat.Models;

namespace AiAgent.Api.Domain.Chat.Interfaces;

public interface IChatService
{
    Task<string> ProcessAsync(ChatRequest request);
}
