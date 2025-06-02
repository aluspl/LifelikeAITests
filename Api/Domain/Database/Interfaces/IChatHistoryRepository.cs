using Api.Domain.Database.Entites;

namespace Api.Domain.Database.Interfaces;

public interface IChatHistoryRepository : IRepository
{
    Task InsertAsync(ChatHistoryEntity history);
    Task<List<ChatHistoryEntity>> GetChatHistoryAsync();
}

public interface IRepository
{
}