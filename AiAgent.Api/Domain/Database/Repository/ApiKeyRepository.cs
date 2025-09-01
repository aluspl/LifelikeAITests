using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using MongoDB.Driver;

namespace AiAgent.Api.Domain.Database.Repository;

public class ApiKeyRepository(IMongoDatabase database) : Repository<ApiKey>(database, "ApiKeys"), IApiKeyRepository
{
    public async Task<ApiKey> GetByKeyAsync(string key)
    {
        return await Collection.Find(x => x.Key == key).FirstOrDefaultAsync();
    }
}
