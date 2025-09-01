using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.StepCache.Models;

namespace AiAgent.Api.Domain.StepCache.Queries;

public class GetStepCachesByAgentIdQuery : IQuery<ICollection<StepCacheResponse>>
{
    public Guid AgentId { get; set; }
}