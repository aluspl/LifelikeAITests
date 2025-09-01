using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AiAgent.Api.Domain.Agents.Commands;
using AiAgent.Api.Domain.Agents.CommandHandlers;
using AiAgent.Api.Domain.Agents.Queries;
using AiAgent.Api.Domain.Agents.QueryHandlers;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Agents.Models;

namespace AiAgent.Api.Tests.Domain.Agents;

public class AgentHandlersTests
{
    [Fact]
    public async Task CreateAgentCommandHandler_ShouldInsertAgent()
    {
        // Arrange
        var mockAgentRepository = new Mock<IAgentRepository>();
        var handler = new CreateAgentCommandHandler(mockAgentRepository.Object);
        var request = new CreateAgentRequest
        {
            Name = "Test Agent",
            Description = "Test Description"
        };
        var command = new CreateAgentCommand { Request = request };

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockAgentRepository.Verify(repo => repo.InsertAsync(It.Is<AgentEntity>(a => 
            a.Name == request.Name && 
            a.Description == request.Description
        )), Times.Once);
    }

    [Fact]
    public async Task GetAllAgentsQueryHandler_ShouldReturnAllAgents()
    {
        // Arrange
        var mockAgentRepository = new Mock<IAgentRepository>();
        var expectedAgents = new List<AgentEntity>
        {
            new AgentEntity { Id = Guid.NewGuid(), Name = "Agent 1" },
            new AgentEntity { Id = Guid.NewGuid(), Name = "Agent 2" }
        };
        mockAgentRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedAgents);

        var handler = new GetAllAgentsQueryHandler(mockAgentRepository.Object);
        var query = new GetAllAgentsQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAgents.Count, result.Count());
        Assert.Contains(expectedAgents[0], result);
        Assert.Contains(expectedAgents[1], result);
        mockAgentRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}
