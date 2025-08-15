using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AiAgent.Api.Domain.Agents.Orchestration;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Knowledge.Enums;
using AiAgent.Api.Domain.Knowledge.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Xunit;

namespace Api.IntegrationTests
{
    public class AgentsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AgentsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task ExecuteAgent_ShouldReturnSuccessAndCorrectResult_WhenAgentExists()
        {
            // Arrange
            var agentId = "test-agent-id";
            var step1Id = "step1-id";
            var instructionId = "instr-id";
            var knowledgeId = "know-id";
            var expectedAiResponse = "{\"content\":\"AI processed result\"}";

            // Setup AgentEntity
            var agent = new AgentEntity
            {
                Id = agentId,
                Name = "Test Agent",
                StepIds = new List<string> { step1Id }
            };
            _factory.AgentRepositoryMock.Setup(r => r.GetByIdAsync(agentId)).ReturnsAsync(agent);

            // Setup AgentStepEntity
            var step1 = new AgentStepEntity
            {
                Id = step1Id,
                Order = 1,
                Name = "Test Step",
                InstructionId = instructionId,
                KnowledgeIds = new List<string> { knowledgeId },
                ModelProvider = AiAgent.Api.Domain.Chat.Enums.AiProvider.Gemini,
                IsCached = false
            };
            _factory.AgentStepRepositoryMock.Setup(r => r.GetByIdAsync(step1Id)).ReturnsAsync(step1);

            // Setup InstructionEntity
            var instruction = new InstructionEntity { Id = instructionId, Content = "Test Instruction Content" };
            _factory.InstructionRepositoryMock.Setup(r => r.GetByIdAsync(instructionId)).ReturnsAsync(instruction);

            // Setup KnowledgeEntity
            var knowledge = new KnowledgeEntity
            {
                Id = knowledgeId,
                Key = "test-knowledge-key",
                Module = "test-module",
                SourceType = KnowledgeSourceType.Inline,
                Items = new List<KnowledgeItem> { new KnowledgeItem { Key = "fact1", Value = "value1" } }
            };
            _factory.KnowledgeRepositoryMock.Setup(r => r.GetByIdAsync(knowledgeId)).ReturnsAsync(knowledge);

            // Setup AI Service
            _factory.AiServiceGeminiMock.Setup(s => s.GetChatCompletionAsync(It.IsAny<string>(), It.IsAny<string>()))
                                       .ReturnsAsync(expectedAiResponse);

            var requestBody = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { { "input", "initial input data" } }
            };
            var jsonPayload = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/api/agents/{agentId}/execute", httpContent);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedAiResponse, stringResponse);

            // Verify interactions
            _factory.AgentRepositoryMock.Verify(r => r.GetByIdAsync(agentId), Times.Once);
            _factory.AgentStepRepositoryMock.Verify(r => r.GetByIdAsync(step1Id), Times.Once);
            _factory.InstructionRepositoryMock.Verify(r => r.GetByIdAsync(instructionId), Times.Once);
            _factory.KnowledgeRepositoryMock.Verify(r => r.GetByIdAsync(knowledgeId), Times.Once);
            _factory.AiServiceGeminiMock.Verify(s => s.GetChatCompletionAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}