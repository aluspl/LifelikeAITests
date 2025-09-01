using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Agents.Commands;

namespace AiAgent.Api.Domain.Agents.CommandHandlers;

public class DeleteAgentCommandHandler(IAgentRepository agentRepository) : ICommandHandler<DeleteAgentCommand>
{
    public async Task HandleAsync(DeleteAgentCommand command)
    {
        await agentRepository.DeleteAsync(command.Id);
    }
}