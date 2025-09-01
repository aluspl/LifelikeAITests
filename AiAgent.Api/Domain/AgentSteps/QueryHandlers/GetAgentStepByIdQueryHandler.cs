using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.AgentSteps.Queries;
using AiAgent.Api.Infrastructure.CQRS.Interfaces;

namespace AiAgent.Api.Domain.AgentSteps.QueryHandlers;

public class GetAgentStepByIdQueryHandler(IAgentStepRepository agentStepRepository)
    : IQueryHandler<GetAgentStepByIdQuery, AgentStepEntity>
{
    public async Task<AgentStepEntity> HandleAsync(GetAgentStepByIdQuery query)
    {
        return await agentStepRepository.GetByIdAsync(query.Id);
    }
}