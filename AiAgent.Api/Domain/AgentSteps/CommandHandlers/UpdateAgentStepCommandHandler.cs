using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.AgentSteps.Commands;

namespace AiAgent.Api.Domain.AgentSteps.CommandHandlers;

public class UpdateAgentStepCommandHandler(IAgentStepRepository agentStepRepository)
    : ICommandHandler<UpdateAgentStepCommand>
{
    public async Task HandleAsync(UpdateAgentStepCommand command)
    {
        var existingAgentStep = await agentStepRepository.GetByIdAsync(command.Id);
        if (existingAgentStep == null)
        {
            throw new ArgumentException($"Agent Step with ID {command.Id} not found.");
        }

        existingAgentStep.Order = command.Request.Order;
        existingAgentStep.Name = command.Request.Name;
        existingAgentStep.Instruction = command.Request.Instruction;
        existingAgentStep.KnowledgeIds = command.Request.KnowledgeIds;
        existingAgentStep.ModelProvider = command.Request.ModelProvider;
        existingAgentStep.IsCached = command.Request.IsCached;

        await agentStepRepository.UpdateAsync(existingAgentStep);
    }
}