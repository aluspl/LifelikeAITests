using AiAgent.Api.Domain.StepCache.Models;
using AiAgent.Api.Domain.Common.Interfaces;

namespace AiAgent.Api.Domain.StepCache.Interfaces;

public interface IStepCacheService : IService
{
    Task<StepCacheResponse> GetStepCache(Guid agentId, string query);
    Task<ICollection<StepCacheResponse>> GetAllStepCaches();
    Task<ICollection<StepCacheResponse>> GetStepCaches(Guid agentId);
}
