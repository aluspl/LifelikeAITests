using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using MongoDB.Driver;

namespace AiAgent.Api.Domain.Database.Repository;

public class ApiKeyRepository : Repository<ApiKey>, IApiKeyRepository
{
    public ApiKeyRepository(IMongoDatabase database) : base(database, "ApiKeys")
    {
    }

    public async Task<ApiKey?> GetByKeyAsync(string key)
    {
        return await Collection.Find(x => x.Key == key).FirstOrDefaultAsync();
    }
}
