using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Database.Interfaces;

public interface IStepCacheRepository : IRepository<StepCacheEntity>
{
    Task<StepCacheEntity> GetByModuleAsync(string module);
    Task<StepCacheEntity> GetByKeyAsync(string key);
}
