using AiAgent.Api.Domain.Configuration;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AiAgent.Api.Domain.Database.Repository;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly IMongoCollection<TEntity> Collection;

    public Repository(IMongoDatabase database, string collectionName)
    {
        Collection = database.GetCollection<TEntity>(collectionName);
    }

    public async Task InsertAsync(TEntity entity)
    {
        await Collection.InsertOneAsync(entity);
    }

    public async Task<TEntity> GetByIdAsync(string id)
    {
        return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Collection.Find(_ => true).ToListAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        await Collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
    }

    public async Task DeleteAsync(string id)
    {
        await Collection.DeleteOneAsync(x => x.Id == id);
    }
}
