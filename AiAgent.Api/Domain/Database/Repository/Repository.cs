using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace AiAgent.Api.Domain.Database.Repository;

public class Repository<TEntity>(IMongoDatabase database, string collectionName) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly IMongoCollection<TEntity> Collection = database.GetCollection<TEntity>(collectionName);

    public async Task InsertAsync(TEntity entity)
    {
        await Collection.InsertOneAsync(entity);
    }

    public async Task<TEntity> GetByIdAsync(Guid id)
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

    public async Task DeleteAsync(Guid id)
    {
        await Collection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }
}
