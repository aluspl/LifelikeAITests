using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Commands;

namespace AiAgent.Api.Domain.Knowledge.CommandHandlers;

public class DeleteKnowledgeCommandHandler(IKnowledgeRepository knowledgeRepository)
    : ICommandHandler<DeleteKnowledgeCommand>
{
    public async Task HandleAsync(DeleteKnowledgeCommand command)
    {
        await knowledgeRepository.DeleteAsync(command.Id);
    }
}