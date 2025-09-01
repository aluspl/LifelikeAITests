using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Agents.Queries;

namespace AiAgent.Api.Domain.Agents.QueryHandlers;

public class GetAllAgentsQueryHandler(IAgentRepository agentRepository)
    : IQueryHandler<GetAllAgentsQuery, IEnumerable<AgentEntity>>
{
    public async Task<IEnumerable<AgentEntity>> HandleAsync(GetAllAgentsQuery query)
    {
        return await agentRepository.GetAllAsync();
    }
}