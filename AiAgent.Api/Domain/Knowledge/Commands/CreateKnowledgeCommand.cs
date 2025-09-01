using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Knowledge.Models;

namespace AiAgent.Api.Domain.Knowledge.Commands;

public class CreateKnowledgeCommand : ICommand<Guid>
{
    public CreateKnowledgeRequest Request { get; set; }
}