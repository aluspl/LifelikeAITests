using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Agents.Models;

namespace AiAgent.Api.Domain.Agents.Commands;

public class CreateAgentCommand : ICommand
{
    public CreateAgentRequest Request { get; set; }
}