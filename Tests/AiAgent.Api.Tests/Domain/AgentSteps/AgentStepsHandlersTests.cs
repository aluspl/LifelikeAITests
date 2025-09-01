using Xunit;
using Moq;
using System.Threading.Tasks;
using AiAgent.Api.Domain.AgentSteps.Queries;
using AiAgent.Api.Domain.AgentSteps.QueryHandlers;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Tests.Domain.AgentSteps;

public class AgentStepsHandlersTests
{
    [Fact]
    public async Task GetAgentStepByIdQueryHandler_ShouldReturnAgentStep_WhenExists()
    {
        // Arrange
        var mockAgentStepRepository = new Mock<IAgentStepRepository>();
        var agentStepId = Guid.NewGuid();
        var expectedAgentStep = new AgentStepEntity { Id = agentStepId, Name = "Test Step" };
        mockAgentStepRepository.Setup(repo => repo.GetByIdAsync(agentStepId)).ReturnsAsync(expectedAgentStep);

        var handler = new GetAgentStepByIdQueryHandler(mockAgentStepRepository.Object);
        var query = new GetAgentStepByIdQuery { Id = agentStepId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAgentStep.Id, result.Id);
        mockAgentStepRepository.Verify(repo => repo.GetByIdAsync(agentStepId), Times.Once);
    }

    [Fact]
    public async Task GetAgentStepByIdQueryHandler_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var mockAgentStepRepository = new Mock<IAgentStepRepository>();
        var agentStepId = Guid.NewGuid();
        mockAgentStepRepository.Setup(repo => repo.GetByIdAsync(agentStepId)).ReturnsAsync((AgentStepEntity)null);

        var handler = new GetAgentStepByIdQueryHandler(mockAgentStepRepository.Object);
        var query = new GetAgentStepByIdQuery { Id = agentStepId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
        mockAgentStepRepository.Verify(repo => repo.GetByIdAsync(agentStepId), Times.Once);
    }
}
