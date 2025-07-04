using Api.Domain.Chat.Models;

namespace Api.Domain.Chat.Interfaces;

public interface IChatService
{
    Task<string> ProcessAsync(ChatRequest request);
}
