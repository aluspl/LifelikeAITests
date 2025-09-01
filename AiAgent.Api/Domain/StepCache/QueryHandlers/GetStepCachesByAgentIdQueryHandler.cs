using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.StepCache.Interfaces;
using AiAgent.Api.Domain.StepCache.Models;
using AiAgent.Api.Domain.StepCache.Queries;

namespace AiAgent.Api.Domain.StepCache.QueryHandlers;

public class GetStepCachesByAgentIdQueryHandler(IStepCacheService stepCacheService)
    : IQueryHandler<GetStepCachesByAgentIdQuery, ICollection<StepCacheResponse>>
{
    public async Task<ICollection<StepCacheResponse>> HandleAsync(GetStepCachesByAgentIdQuery query)
    {
        return await stepCacheService.GetStepCaches(query.AgentId);
    }
}