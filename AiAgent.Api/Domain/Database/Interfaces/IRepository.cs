using AiAgent.Api.Domain.Database.Entites;
using System.Linq.Expressions;

namespace AiAgent.Api.Domain.Database.Interfaces;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    Task InsertAsync(TEntity entity);
    Task<TEntity> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
    Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter);
}
