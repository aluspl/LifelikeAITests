using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.AgentSteps.Queries;

public class GetAgentStepByIdQuery : IQuery<AgentStepEntity>
{
    public Guid Id { get; set; }
}