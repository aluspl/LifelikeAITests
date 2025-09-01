using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Commands;

namespace AiAgent.Api.Domain.Knowledge.CommandHandlers;

public class CreateKnowledgeCommandHandler(IKnowledgeRepository knowledgeRepository)
    : ICommandHandler<CreateKnowledgeCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateKnowledgeCommand command)
    {
        var knowledge = new KnowledgeEntity
        {
            Key = command.Request.Key,
            Module = command.Request.Module,
            SourceType = command.Request.SourceType,
            Items = command.Request.Items,
            BlobUrl = command.Request.BlobUrl,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };
        await knowledgeRepository.InsertAsync(knowledge);
        return knowledge.Id;
    }
}