using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using MongoDB.Driver;

namespace AiAgent.Api.Domain.Database.Repository;

public class KnowledgeRepository(IMongoDatabase database)
    : Repository<KnowledgeEntity>(database, "Knowledge"), IKnowledgeRepository
{
    public async Task<KnowledgeEntity> GetByKeyAndModuleAsync(string key, string module)
    {
        return await Collection.Find(x => x.Key == key && x.Module == module).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<KnowledgeEntity>> GetByKeysAndModuleAsync(IEnumerable<string> keys, string module)
    {
        var filter = Builders<KnowledgeEntity>.Filter.In(x => x.Key, keys) & Builders<KnowledgeEntity>.Filter.Eq(x => x.Module, module);
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<KnowledgeEntity>> GetAllByModuleAsync(string module)
    {
        return await Collection.Find(x => x.Module == module).ToListAsync();
    }

    public async Task UpsertAsync(KnowledgeEntity entity)
    {
        var existingEntity = await GetByKeyAndModuleAsync(entity.Key, entity.Module);
        if (existingEntity != null)
        {
            entity.Id = existingEntity.Id;
        }

        var filter = Builders<KnowledgeEntity>.Filter.Where(x => x.Key == entity.Key && x.Module == entity.Module);
        var options = new ReplaceOptions { IsUpsert = true };
        await Collection.ReplaceOneAsync(filter, entity, options);
    }
}
