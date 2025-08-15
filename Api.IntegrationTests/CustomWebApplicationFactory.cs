using AiAgent.Api;
using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.AI.Services;
using AiAgent.Api.Domain.Agents.Orchestration;
using AiAgent.Api.Domain.Common.Interfaces;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Instructions.Interfaces;
using AiAgent.Api.Domain.Instructions.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<AiAgent.Api.Program>
{
    public Mock<IAgentOrchestratorService> AgentOrchestratorServiceMock { get; }
    public Mock<IKnowledgeRepository> KnowledgeRepositoryMock { get; }
    public Mock<IAgentRepository> AgentRepositoryMock { get; }
    public Mock<IAgentStepRepository> AgentStepRepositoryMock { get; }
    public Mock<IInstructionRepository> InstructionRepositoryMock { get; }
    public Mock<IAiService> AiServiceGeminiMock { get; }
    public Mock<IInstructionService> InstructionServiceMock { get; } // Added

    public CustomWebApplicationFactory()
    {
        AgentOrchestratorServiceMock = new Mock<IAgentOrchestratorService>();
        KnowledgeRepositoryMock = new Mock<IKnowledgeRepository>();
        AgentRepositoryMock = new Mock<IAgentRepository>();
        AgentStepRepositoryMock = new Mock<IAgentStepRepository>();
        InstructionRepositoryMock = new Mock<IInstructionRepository>();
        AiServiceGeminiMock = new Mock<IAiService>();
        InstructionServiceMock = new Mock<IInstructionService>(); // Initialized
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all services that will be replaced by mocks or test-specific implementations
            var servicesToRemove = services.Where(d => 
                d.ServiceType.Name.EndsWith("Repository") || 
                d.ServiceType == typeof(IMongoClient) ||
                d.ServiceType == typeof(IMongoDatabase) ||
                d.ServiceType == typeof(IDataSeederService) ||
                d.ServiceType == typeof(IAgentOrchestratorService) ||
                d.ServiceType == typeof(IAiService) || 
                d.ServiceType == typeof(IInstructionService) 
            ).ToList();

            foreach (var descriptor in servicesToRemove)
            {
                services.Remove(descriptor);
            }

            // --- Register Mocks and Test Implementations ---

            // 1. Use the empty seeder for tests
            services.AddScoped<IDataSeederService, TestScopedDataSeeder>();

            // 2. Mock the entire database layer
            var mockMongoClient = new Mock<IMongoClient>();
            var mockMongoDatabase = new Mock<IMongoDatabase>();
            mockMongoClient.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(mockMongoDatabase.Object);
            services.AddSingleton<IMongoClient>(mockMongoClient.Object);
            services.AddSingleton<IMongoDatabase>(mockMongoDatabase.Object);

            // 3. Mock all repositories
            services.AddScoped<IInstructionRepository>(_ => InstructionRepositoryMock.Object);
            services.AddScoped<IKnowledgeRepository>(_ => KnowledgeRepositoryMock.Object);
            services.AddScoped<IAgentRepository>(_ => AgentRepositoryMock.Object);
            services.AddScoped<IExecutionLogRepository>(_ => new Mock<IExecutionLogRepository>().Object);
            services.AddScoped<IApiKeyRepository>(_ => new Mock<IApiKeyRepository>().Object);
            services.AddScoped<IAgentStepRepository>(_ => AgentStepRepositoryMock.Object);

            // 4. Mock high-level services used in tests
            services.AddScoped<IAgentOrchestratorService>(_ => AgentOrchestratorServiceMock.Object);
            services.AddScoped<IAiService>(_ => new MockAiService()); // Direct implementation
            // Removed OpenAiService registration as it's not used in current tests
            services.AddScoped<IInstructionService>(_ => InstructionServiceMock.Object); // Changed to use InstructionServiceMock
        });
    }

    // Simple mock AI service implementation
    private class MockAiService : IAiService
    {
        public AiAgent.Api.Domain.Chat.Enums.AiProvider Provider => AiAgent.Api.Domain.Chat.Enums.AiProvider.Gemini;

        public Task<string> GetChatCompletionAsync(string userMessage, string instructions)
        {
            return Task.FromResult("{\"content\":\"AI processed result\"}");
        }
    }
}
