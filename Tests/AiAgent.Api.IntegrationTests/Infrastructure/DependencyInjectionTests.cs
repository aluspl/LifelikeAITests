using AiAgent.Api.Domain.Agents.Commands;
using AiAgent.Api.Domain.Agents.Queries;
using AiAgent.Api.Domain.AgentSteps.Commands;
using AiAgent.Api.Domain.AgentSteps.Queries;
using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Chat.Enums;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Commands;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using AiAgent.Api.Domain.Knowledge.Queries;
using AiAgent.Api.Domain.StepCache.Queries;
using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Api.IntegrationTests.Infrastructure;

public class DependencyInjectionTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public void All_Registered_Services_Can_Be_Resolved()
    {
        using var scope = factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Test core services
        Assert.NotNull(serviceProvider.GetService<IMongoClient>());
        Assert.NotNull(serviceProvider.GetService<IMongoDatabase>());
        Assert.NotNull(serviceProvider.GetService<IMediator>());

        // Test AI Providers
        Assert.NotNull(serviceProvider.GetKeyedService<IAiProvider>(nameof(AiProvider.OpenAi).ToLower()));
        Assert.NotNull(serviceProvider.GetKeyedService<IAiProvider>(nameof(AiProvider.Gemini).ToLower()));

        // Test Repositories (a few examples)
        Assert.NotNull(serviceProvider.GetService<IAgentRepository>());
        Assert.NotNull(serviceProvider.GetService<IAgentStepRepository>());
        Assert.NotNull(serviceProvider.GetService<IKnowledgeRepository>());
        Assert.NotNull(serviceProvider.GetService<IApiKeyRepository>());
        Assert.NotNull(serviceProvider.GetService<IExecutionLogRepository>());

        // Test Command Handlers (a few examples)
        Assert.NotNull(serviceProvider.GetService<ICommandHandler<CreateAgentCommand>>());
        Assert.NotNull(serviceProvider.GetService<ICommandHandler<DeleteAgentCommand>>());
        Assert.NotNull(serviceProvider.GetService<ICommandHandler<CreateAgentStepCommand>>());
        Assert.NotNull(serviceProvider.GetService<ICommandHandler<DeleteAgentStepCommand>>());
        Assert.NotNull(serviceProvider.GetService<ICommandHandler<CreateKnowledgeCommand>>());
        Assert.NotNull(serviceProvider.GetService<ICommandHandler<DeleteKnowledgeCommand>>());

        // Test Query Handlers (a few examples)
        Assert.NotNull(serviceProvider.GetService<IQueryHandler<GetAllAgentsQuery, IEnumerable<AiAgent.Api.Domain.Database.Entites.AgentEntity>>>());
        Assert.NotNull(serviceProvider.GetService<IQueryHandler<GetAgentByIdQuery, AiAgent.Api.Domain.Database.Entites.AgentEntity>>());
        Assert.NotNull(serviceProvider.GetService<IQueryHandler<GetAllAgentStepsQuery, IEnumerable<AiAgent.Api.Domain.Database.Entites.AgentStepEntity>>>());
        Assert.NotNull(serviceProvider.GetService<IQueryHandler<GetAgentStepByIdQuery, AiAgent.Api.Domain.Database.Entites.AgentStepEntity>>());
        Assert.NotNull(serviceProvider.GetService<IQueryHandler<GetAllKnowledgeQuery, IEnumerable<AiAgent.Api.Domain.Database.Entites.KnowledgeEntity>>>());
        Assert.NotNull(serviceProvider.GetService<IQueryHandler<GetKnowledgeByIdQuery, AiAgent.Api.Domain.Database.Entites.KnowledgeEntity>>());
        Assert.NotNull(serviceProvider.GetService<IQueryHandler<GetAllStepCachesQuery, ICollection<AiAgent.Api.Domain.StepCache.Models.StepCacheResponse>>>());
        Assert.NotNull(serviceProvider.GetService<IQueryHandler<GetStepCachesByAgentIdQuery, ICollection<AiAgent.Api.Domain.StepCache.Models.StepCacheResponse>>>());

        // Test other services
        Assert.NotNull(serviceProvider.GetService<IDataKnowledgeService>());
        Assert.NotNull(serviceProvider.GetService<AiAgent.Api.Domain.Agents.Orchestration.Interfaces.IAgentOrchestratorService>());
        Assert.NotNull(serviceProvider.GetService<AiAgent.Api.Domain.Common.Interfaces.IDataSeederService>());
    }
}