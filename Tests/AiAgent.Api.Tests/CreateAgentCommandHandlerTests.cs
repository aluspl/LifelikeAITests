using Moq;
using AiAgent.Api.Domain.Agents.Commands;
using AiAgent.Api.Domain.Agents.CommandHandlers;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Agents.Models;

namespace AiAgent.Api.Tests;

public class CreateAgentCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldInsertAgent_WhenCalledWithValidCommand()
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
}