using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.AgentSteps.Commands;

namespace AiAgent.Api.Domain.AgentSteps.CommandHandlers;

public class DeleteAgentStepCommandHandler(IAgentStepRepository agentStepRepository)
    : ICommandHandler<DeleteAgentStepCommand>
{
    public async Task HandleAsync(DeleteAgentStepCommand command)
    {
        await agentStepRepository.DeleteAsync(command.Id);
    }
}