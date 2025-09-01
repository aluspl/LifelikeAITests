using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Knowledge.Models;

namespace AiAgent.Api.Domain.Knowledge.Commands;

public class UpdateKnowledgeCommand : ICommand
{
    public Guid Id { get; set; }
    public UpdateKnowledgeRequest Request { get; set; }
}