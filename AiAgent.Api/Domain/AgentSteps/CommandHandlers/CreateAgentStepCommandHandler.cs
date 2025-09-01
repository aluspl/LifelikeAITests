using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.AgentSteps.Commands;
using AiAgent.Api.Infrastructure.CQRS.Interfaces;

namespace AiAgent.Api.Domain.AgentSteps.CommandHandlers;

public class CreateAgentStepCommandHandler(IAgentStepRepository agentStepRepository)
    : ICommandHandler<CreateAgentStepCommand>
{
    public async Task HandleAsync(CreateAgentStepCommand command)
    {
        var agentStep = new AgentStepEntity
        {
            Order = command.Request.Order,
            Name = command.Request.Name,
            Instruction = command.Request.Instruction,
            KnowledgeIds = command.Request.KnowledgeIds,
            ModelProvider = command.Request.ModelProvider,
            IsCached = command.Request.IsCached
        };
        await agentStepRepository.InsertAsync(agentStep);
    }
}