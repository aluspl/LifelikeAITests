using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.AgentSteps.Models;

namespace AiAgent.Api.Domain.AgentSteps.Commands;

public class CreateAgentStepCommand : ICommand
{
    public CreateAgentStepRequest Request { get; set; }
}