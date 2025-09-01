using AiAgent.Api.Domain.Common.Interfaces;
using AiAgent.Api.Domain.StepCache.Interfaces;
using AiAgent.Api.Domain.StepCache.Models;

namespace AiAgent.Api.Domain.StepCache.Services;

public class StepCacheService : IStepCacheService, IService
{
    public Task<ICollection<StepCacheResponse>> GetAllStepCaches()
    {
        return Task.FromResult<ICollection<StepCacheResponse>>(new List<StepCacheResponse>());
    }

    public Task<StepCacheResponse> GetStepCache(Guid agentId, string query)
    {
        return Task.FromResult<StepCacheResponse>(null);
    }

    public Task<ICollection<StepCacheResponse>> GetStepCaches(Guid agentId)
    {
        return Task.FromResult<ICollection<StepCacheResponse>>(new List<StepCacheResponse>());
    }
}
