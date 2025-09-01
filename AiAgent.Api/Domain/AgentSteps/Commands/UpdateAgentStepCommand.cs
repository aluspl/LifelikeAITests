using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.AgentSteps.Models;

namespace AiAgent.Api.Domain.AgentSteps.Commands;

public class UpdateAgentStepCommand : ICommand
{
    public Guid Id { get; set; }
    public UpdateAgentStepRequest Request { get; set; }
}