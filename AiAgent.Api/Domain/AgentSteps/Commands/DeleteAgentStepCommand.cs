using AiAgent.Api.Infrastructure.CQRS.Interfaces;

namespace AiAgent.Api.Domain.AgentSteps.Commands;

public class DeleteAgentStepCommand : ICommand
{
    public Guid Id { get; set; }
}