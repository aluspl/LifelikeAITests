using AiAgent.Api.Infrastructure.CQRS.Interfaces;

namespace AiAgent.Api.Domain.Agents.Commands;

public class DeleteAgentCommand : ICommand
{
    public Guid Id { get; set; }
}