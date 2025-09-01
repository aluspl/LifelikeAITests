using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Commands;

namespace AiAgent.Api.Domain.Knowledge.CommandHandlers;

public class UpdateKnowledgeCommandHandler(IKnowledgeRepository knowledgeRepository)
    : ICommandHandler<UpdateKnowledgeCommand>
{
    public async Task HandleAsync(UpdateKnowledgeCommand command)
    {
        var existingKnowledge = await knowledgeRepository.GetByIdAsync(command.Id);
        if (existingKnowledge == null)
        {
            throw new ArgumentException($"Knowledge with ID {command.Id} not found.");
        }

        existingKnowledge.Key = command.Request.Key;
        existingKnowledge.Module = command.Request.Module;
        existingKnowledge.SourceType = command.Request.SourceType;
        existingKnowledge.Items = command.Request.Items;
        existingKnowledge.BlobUrl = command.Request.BlobUrl;
        existingKnowledge.Updated = DateTime.UtcNow;

        await knowledgeRepository.UpdateAsync(existingKnowledge);
    }
}