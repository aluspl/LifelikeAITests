using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Database.Interfaces;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    Task InsertAsync(TEntity entity);
    Task<TEntity> GetByIdAsync(string id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(string id);
}
