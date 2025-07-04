using Api.Domain.Configuration;
using Api.Domain.Database.Entites;
using Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Domain.Database.Repository;

public class ChatHistoryRepository : Repository<ChatHistoryEntity>, IChatHistoryRepository
{
    public ChatHistoryRepository(IMongoDatabase database) : base(database, "ChatHistory")
    {
    }

    public async Task<List<ChatHistoryEntity>> GetChatHistoryAsync()
    {
        var history = await GetAllAsync();
        return history.ToList();
    }
}