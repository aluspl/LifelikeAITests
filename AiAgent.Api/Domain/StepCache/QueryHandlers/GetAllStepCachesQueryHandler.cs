using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.StepCache.Interfaces;
using AiAgent.Api.Domain.StepCache.Models;
using AiAgent.Api.Domain.StepCache.Queries;

namespace AiAgent.Api.Domain.StepCache.QueryHandlers;

public class GetAllStepCachesQueryHandler(IStepCacheService stepCacheService)
    : IQueryHandler<GetAllStepCachesQuery, ICollection<StepCacheResponse>>
{
    public async Task<ICollection<StepCacheResponse>> HandleAsync(GetAllStepCachesQuery query)
    {
        return await stepCacheService.GetAllStepCaches();
    }
}