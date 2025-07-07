using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Database.Interfaces;

public interface IChatHistoryRepository : IRepository
{
    Task InsertAsync(ChatHistoryEntity history);
    Task<List<ChatHistoryEntity>> GetChatHistoryAsync();
}

public interface IRepository
{
}