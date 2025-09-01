using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Agents.Queries;

namespace AiAgent.Api.Domain.Agents.QueryHandlers;

public class GetAgentByIdQueryHandler(IAgentRepository agentRepository) : IQueryHandler<GetAgentByIdQuery, AgentEntity>
{
    public async Task<AgentEntity> HandleAsync(GetAgentByIdQuery query)
    {
        return await agentRepository.GetByIdAsync(query.Id);
    }
}