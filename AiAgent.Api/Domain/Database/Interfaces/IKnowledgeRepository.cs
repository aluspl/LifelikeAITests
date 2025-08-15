using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Database.Interfaces;

public interface IKnowledgeRepository : IRepository<KnowledgeEntity>
{
    Task<KnowledgeEntity> GetByKeyAndModuleAsync(string key, string module);
    Task<IEnumerable<KnowledgeEntity>> GetByKeysAndModuleAsync(IEnumerable<string> keys, string module);
    Task<IEnumerable<KnowledgeEntity>> GetAllByModuleAsync(string module);
    Task UpsertAsync(KnowledgeEntity entity);
}
