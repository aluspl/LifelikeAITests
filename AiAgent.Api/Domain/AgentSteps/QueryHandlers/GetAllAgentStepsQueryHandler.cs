using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.AgentSteps.Queries;

namespace AiAgent.Api.Domain.AgentSteps.QueryHandlers;

public class GetAllAgentStepsQueryHandler(IAgentStepRepository agentStepRepository)
    : IQueryHandler<GetAllAgentStepsQuery, IEnumerable<AgentStepEntity>>
{
    public async Task<IEnumerable<AgentStepEntity>> HandleAsync(GetAllAgentStepsQuery query)
    {
        return await agentStepRepository.GetAllAsync();
    }
}