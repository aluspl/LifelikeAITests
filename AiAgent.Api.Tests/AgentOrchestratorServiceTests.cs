using Xunit;
using Moq;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Agents.Orchestration;
using System.Collections.Generic;
using System.Threading.Tasks;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Chat.Enums;
using System;
using AiAgent.Api.Domain.Knowledge.Enums;
using AiAgent.Api.Domain.Knowledge.Models;

namespace AiAgent.Api.Tests
{
    public class AgentOrchestratorServiceTests
    {
        private readonly Mock<IAgentRepository> _agentRepositoryMock;
        private readonly Mock<IAgentStepRepository> _agentStepRepositoryMock;
        private readonly Mock<IInstructionRepository> _instructionRepositoryMock;
        private readonly Mock<IKnowledgeRepository> _knowledgeRepositoryMock;
        private readonly Mock<IExecutionLogRepository> _executionLogRepositoryMock;
        private readonly Mock<IAiService> _geminiServiceMock;
        private readonly AgentOrchestratorService _orchestratorService;

        public AgentOrchestratorServiceTests()
        {
            _agentRepositoryMock = new Mock<IAgentRepository>();
            _agentStepRepositoryMock = new Mock<IAgentStepRepository>();
            _instructionRepositoryMock = new Mock<IInstructionRepository>();
            _knowledgeRepositoryMock = new Mock<IKnowledgeRepository>();
            _executionLogRepositoryMock = new Mock<IExecutionLogRepository>();
            _geminiServiceMock = new Mock<IAiService>();

            _geminiServiceMock.Setup(s => s.Provider).Returns(AiProvider.Gemini);
            var aiServices = new List<IAiService> { _geminiServiceMock.Object };

            _orchestratorService = new AgentOrchestratorService(
                _agentRepositoryMock.Object,
                _agentStepRepositoryMock.Object,
                _instructionRepositoryMock.Object,
                _knowledgeRepositoryMock.Object,
                aiServices,
                _executionLogRepositoryMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAgent_ShouldFollowStepsAndReturnFinalOutput_WhenAgentIsValid()
        {
            // Arrange
            var agentId = "test-agent-id";
            var initialInput = "{\"team1\":\"Team A\"}";
            var instructionId = "instr-id";
            var knowledgeId = "know-id";
            var step1Id = "step1-id";
            var step2Id = "step2-id";

            var agent = new AgentEntity
            {
                Id = agentId,
                Name = "Test Agent",
                StepIds = new List<string> { step1Id, step2Id }
            };

            var step1 = new AgentStepEntity { Id = step1Id, Order = 1, Name = "Step 1", InstructionId = instructionId, KnowledgeIds = new List<string> { knowledgeId }, ModelProvider = AiProvider.Gemini, IsCached = false };
            var step2 = new AgentStepEntity { Id = step2Id, Order = 2, Name = "Step 2", InstructionId = instructionId, KnowledgeIds = new List<string> { knowledgeId }, ModelProvider = AiProvider.Gemini, IsCached = false };

            var instruction = new InstructionEntity { Id = instructionId, Key = "TestInstructionKey", Content = "Test Instruction" };
            var knowledge = new KnowledgeEntity { Id = knowledgeId, Key = "TestKnowledgeKey", Module = "TestModule", SourceType = KnowledgeSourceType.Inline, Items = new List<KnowledgeItem> { new KnowledgeItem { Key = "test", Value = "Test Knowledge" } } };

            var step1OutputContent = "{\"analysis\":\"Step 1 Done\"}";
            var step2OutputContent = "{\"final_analysis\":\"All Steps Done\"}";

            _agentRepositoryMock.Setup(r => r.GetByIdAsync(agentId)).ReturnsAsync(agent);
            _agentStepRepositoryMock.Setup(r => r.GetByIdAsync(step1Id)).ReturnsAsync(step1);
            _agentStepRepositoryMock.Setup(r => r.GetByIdAsync(step2Id)).ReturnsAsync(step2);
            _instructionRepositoryMock.Setup(r => r.GetByIdAsync(instructionId)).ReturnsAsync(instruction);
            _knowledgeRepositoryMock.Setup(r => r.GetByIdAsync(knowledgeId)).ReturnsAsync(knowledge);
            _executionLogRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<ExecutionLogEntity>())).Returns(Task.CompletedTask);

            _geminiServiceMock.SetupSequence(s => s.GetChatCompletionAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(step1OutputContent)
                .ReturnsAsync(step2OutputContent);

            // Act
            var finalResult = await _orchestratorService.ExecuteAgent(agentId, initialInput);

            // Assert
            Assert.Equal(step2OutputContent, finalResult);

            _agentRepositoryMock.Verify(r => r.GetByIdAsync(agentId), Times.Once);
            _agentStepRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Exactly(2));
            _instructionRepositoryMock.Verify(r => r.GetByIdAsync(instructionId), Times.Exactly(2));
            _knowledgeRepositoryMock.Verify(r => r.GetByIdAsync(knowledgeId), Times.Exactly(2));
            _geminiServiceMock.Verify(s => s.GetChatCompletionAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            _executionLogRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<ExecutionLogEntity>()), Times.Once);
        }
    }
}