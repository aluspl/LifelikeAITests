using AiAgent.Api.Infrastructure.CQRS.Interfaces;

namespace AiAgent.Api.Domain.Knowledge.Commands;

public class DeleteKnowledgeCommand : ICommand
{
    public Guid Id { get; set; }
}