using Api.Domain.Configuration;
using Api.Domain.Database.Entites;
using Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Domain.Database.Repository;

public class ChatHistoryRepository : Repository, IChatHistoryRepository
{
    private readonly IMongoCollection<ChatHistoryEntity> _collection;

    public ChatHistoryRepository(IOptions<MongoSettings> mongoSettings) : base(mongoSettings)
    {
        _collection = Database.GetCollection<ChatHistoryEntity>(nameof(ChatHistoryEntity));
    }

    public async Task InsertAsync(ChatHistoryEntity history)
    {
        await _collection.InsertOneAsync(history);
    }

    public async Task<List<ChatHistoryEntity>> GetChatHistoryAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }
}