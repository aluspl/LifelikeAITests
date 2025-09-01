using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Agents.Queries;

public class GetAgentByIdQuery : IQuery<AgentEntity>
{
    public Guid Id { get; set; }
}