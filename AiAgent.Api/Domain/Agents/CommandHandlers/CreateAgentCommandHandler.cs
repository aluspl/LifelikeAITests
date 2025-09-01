using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Agents.Commands;

namespace AiAgent.Api.Domain.Agents.CommandHandlers;

public class CreateAgentCommandHandler(IAgentRepository agentRepository) : ICommandHandler<CreateAgentCommand>
{
    public async Task HandleAsync(CreateAgentCommand command)
    {
        var agent = new AgentEntity
        {
            Name = command.Request.Name,
            Description = command.Request.Description
        };
        await agentRepository.InsertAsync(agent);
    }
}