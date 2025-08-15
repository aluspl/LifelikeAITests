using Xunit;
using Moq;
using AiAgent.Api.Controllers;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AiAgent.Api.Tests
{
    public class AgentStepsControllerTests
    {
        private readonly Mock<IAgentStepRepository> _mockRepo;
        private readonly AgentStepsController _controller;

        public AgentStepsControllerTests()
        {
            _mockRepo = new Mock<IAgentStepRepository>();
            _controller = new AgentStepsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAgentSteps_ShouldReturnAllSteps()
        {
            // Arrange
            var steps = new List<AgentStepEntity> { new AgentStepEntity(), new AgentStepEntity() };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(steps);

            // Act
            var result = await _controller.GetAgentSteps();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            var returnedSteps = Assert.IsAssignableFrom<IEnumerable<AgentStepEntity>>(okResult.Value);
            Assert.Equal(2, new List<AgentStepEntity>(returnedSteps).Count);
        }

        [Fact]
        public async Task GetAgentStep_ShouldReturnStep_WhenStepExists()
        {
            // Arrange
            var stepId = "test-id";
            var step = new AgentStepEntity { Id = stepId, Name = "Test Step" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(stepId)).ReturnsAsync(step);

            // Act
            var result = await _controller.GetAgentStep(stepId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedStep = Assert.IsType<AgentStepEntity>(okResult.Value);
            Assert.Equal(stepId, returnedStep.Id);
        }

        [Fact]
        public async Task GetAgentStep_ShouldReturnNotFound_WhenStepDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((AgentStepEntity)null);

            // Act
            var result = await _controller.GetAgentStep("non-existent-id");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateAgentStep_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var newStep = new AgentStepEntity { Name = "New Step" };

            // Act
            var result = await _controller.CreateAgentStep(newStep);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(AgentStepsController.GetAgentStep), createdAtActionResult.ActionName);
            _mockRepo.Verify(repo => repo.InsertAsync(newStep), Times.Once);
        }

        [Fact]
        public async Task UpdateAgentStep_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var stepId = "test-id";
            var stepToUpdate = new AgentStepEntity { Id = stepId, Name = "Updated Step" };

            // Act
            var result = await _controller.UpdateAgentStep(stepId, stepToUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(repo => repo.UpdateAsync(stepToUpdate), Times.Once);
        }

        [Fact]
        public async Task DeleteAgentStep_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var stepId = "test-id";

            // Act
            var result = await _controller.DeleteAgentStep(stepId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(repo => repo.DeleteAsync(stepId), Times.Once);
        }
    }
}
