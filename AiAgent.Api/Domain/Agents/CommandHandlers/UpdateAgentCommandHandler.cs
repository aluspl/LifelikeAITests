using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Agents.Commands;

namespace AiAgent.Api.Domain.Agents.CommandHandlers;

public class UpdateAgentCommandHandler(IAgentRepository agentRepository) : ICommandHandler<UpdateAgentCommand>
{
    public async Task HandleAsync(UpdateAgentCommand command)
    {
        var existingAgent = await agentRepository.GetByIdAsync(command.Id);
        if (existingAgent == null)
        {
            throw new ArgumentException($"Agent with ID {command.Id} not found.");
        }

        existingAgent.Name = command.Name;
        existingAgent.Description = command.Description;

        await agentRepository.UpdateAsync(existingAgent);
    }
}