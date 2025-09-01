using Microsoft.Extensions.DependencyInjection;
using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Agents.Commands;
using AiAgent.Api.Domain.Agents.Models;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Agents.Queries;
using AiAgent.Api.Domain.AgentSteps.Commands;
using AiAgent.Api.Domain.AgentSteps.Models;
using AiAgent.Api.Domain.AgentSteps.Queries;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Knowledge.Commands;
using AiAgent.Api.Domain.Knowledge.Models;
using AiAgent.Api.Domain.Knowledge.Queries;
using AiAgent.Api.Domain.StepCache.Queries;
using Microsoft.AspNetCore.Http;

namespace Api.IntegrationTests.Domain;

public class MediatorIntegrationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task CreateAgentCommand_ShouldCreateAgentInDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync(); // Ensure a clean state for the test

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var agentRepository = scope.ServiceProvider.GetRequiredService<IAgentRepository>();

        var request = new CreateAgentRequest
        {
            Name = "Integration Test Agent",
            Description = "Agent created via Mediator integration test"
        };
        var command = new CreateAgentCommand { Request = request };

        // Act
        await mediator.SendAsync(command);

        // Assert
        var createdAgent = await agentRepository.GetOneAsync(a => a.Name == request.Name);
        Assert.NotNull(createdAgent);
        Assert.Equal(request.Name, createdAgent.Name);
        Assert.Equal(request.Description, createdAgent.Description);
    }

    [Fact]
    public async Task GetAllAgentsQuery_ShouldReturnAllAgentsFromDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync(); // Ensure a clean state for the test

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var agentRepository = scope.ServiceProvider.GetRequiredService<IAgentRepository>();

        // Insert some agents directly into the database for the test
        var agent1 = new AgentEntity { Name = "Agent 1", Description = "Desc 1" };
        var agent2 = new AgentEntity { Name = "Agent 2", Description = "Desc 2" };
        await agentRepository.InsertAsync(agent1);
        await agentRepository.InsertAsync(agent2);

        var query = new GetAllAgentsQuery();

        // Act
        var result = await mediator.QueryAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a.Name == agent1.Name);
        Assert.Contains(result, a => a.Name == agent2.Name);
    }

    [Fact]
    public async Task UpdateAgentCommand_ShouldUpdateAgentInDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync(); // Ensure a clean state for the test

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var agentRepository = scope.ServiceProvider.GetRequiredService<IAgentRepository>();

        // Insert an agent to be updated
        var originalAgent = new AgentEntity { Name = "Original Name", Description = "Original Description" };
        await agentRepository.InsertAsync(originalAgent);

        var updateRequest = new UpdateAgentRequest
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };
        var command = new UpdateAgentCommand { Id = originalAgent.Id, Name = updateRequest.Name, Description = updateRequest.Description };

        // Act
        await mediator.SendAsync(command);

        // Assert
        var updatedAgent = await agentRepository.GetByIdAsync(originalAgent.Id);
        Assert.NotNull(updatedAgent);
        Assert.Equal(updateRequest.Name, updatedAgent.Name);
        Assert.Equal(updateRequest.Description, updatedAgent.Description);
    }

    [Fact]
    public async Task GetKnowledgeByIdQuery_ShouldReturnKnowledgeFromDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync(); // Ensure a clean state for the test

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var knowledgeRepository = scope.ServiceProvider.GetRequiredService<IKnowledgeRepository>();

        // Insert a knowledge entity directly into the database for the test
        var knowledgeEntity = new KnowledgeEntity
        {
            Key = "TestKnowledgeKey",
            Module = "TestModule",
            SourceType = AiAgent.Api.Domain.Knowledge.Enums.KnowledgeSourceType.Inline,
            Items = new List<AiAgent.Api.Domain.Knowledge.Models.KnowledgeItem> { new AiAgent.Api.Domain.Knowledge.Models.KnowledgeItem { Key = "fact", Value = "value" } }
        };
        await knowledgeRepository.InsertAsync(knowledgeEntity);

        var query = new AiAgent.Api.Domain.Knowledge.Queries.GetKnowledgeByIdQuery { Id = knowledgeEntity.Id };

        // Act
        var result = await mediator.QueryAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(knowledgeEntity.Id, result.Id);
        Assert.Equal(knowledgeEntity.Key, result.Key);
    }

    [Fact]
    public async Task GetAgentByIdQuery_ShouldReturnAgentFromDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var agentRepository = scope.ServiceProvider.GetRequiredService<IAgentRepository>();

        var agent = new AgentEntity { Name = "Agent to Find", Description = "Description to Find" };
        await agentRepository.InsertAsync(agent);

        var query = new GetAgentByIdQuery { Id = agent.Id };

        // Act
        var result = await mediator.QueryAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(agent.Id, result.Id);
        Assert.Equal(agent.Name, result.Name);
    }

    [Fact]
    public async Task DeleteAgentCommand_ShouldDeleteAgentFromDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var agentRepository = scope.ServiceProvider.GetRequiredService<IAgentRepository>();

        var agentToDelete = new AgentEntity { Name = "Agent to Delete", Description = "Description to Delete" };
        await agentRepository.InsertAsync(agentToDelete);

        var command = new DeleteAgentCommand { Id = agentToDelete.Id };

        // Act
        await mediator.SendAsync(command);

        // Assert
        var deletedAgent = await agentRepository.GetByIdAsync(agentToDelete.Id);
        Assert.Null(deletedAgent);
    }

    [Fact]
    public async Task GetAllAgentStepsQuery_ShouldReturnAllAgentStepsFromDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var agentStepRepository = scope.ServiceProvider.GetRequiredService<IAgentStepRepository>();

        var step1 = new AgentStepEntity { Name = "Step 1", Order = 1, Instruction = "Instr 1", ModelProvider = AiAgent.Api.Domain.Chat.Enums.AiProvider.OpenAi };
        var step2 = new AgentStepEntity { Name = "Step 2", Order = 2, Instruction = "Instr 2", ModelProvider = AiAgent.Api.Domain.Chat.Enums.AiProvider.Gemini };
        await agentStepRepository.InsertAsync(step1);
        await agentStepRepository.InsertAsync(step2);

        var query = new GetAllAgentStepsQuery();

        // Act
        var result = await mediator.QueryAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, s => s.Name == step1.Name);
        Assert.Contains(result, s => s.Name == step2.Name);
    }

    [Fact]
    public async Task UpdateAgentStepCommand_ShouldUpdateAgentStepInDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var agentStepRepository = scope.ServiceProvider.GetRequiredService<IAgentStepRepository>();

        var originalStep = new AgentStepEntity { Name = "Original Step", Order = 1, Instruction = "Original Instr", ModelProvider = AiAgent.Api.Domain.Chat.Enums.AiProvider.OpenAi };
        await agentStepRepository.InsertAsync(originalStep);

        var updateRequest = new UpdateAgentStepRequest
        {
            Name = "Updated Step",
            Order = 2,
            Instruction = "Updated Instruction",
            ModelProvider = AiAgent.Api.Domain.Chat.Enums.AiProvider.Gemini,
            IsCached = true
        };
        var command = new UpdateAgentStepCommand { Id = originalStep.Id, Request = updateRequest };

        // Act
        await mediator.SendAsync(command);

        // Assert
        var updatedStep = await agentStepRepository.GetByIdAsync(originalStep.Id);
        Assert.NotNull(updatedStep);
        Assert.Equal(updateRequest.Name, updatedStep.Name);
        Assert.Equal(updateRequest.Order, updatedStep.Order);
        Assert.Equal(updateRequest.Instruction, updatedStep.Instruction);
        Assert.Equal(updateRequest.ModelProvider, updatedStep.ModelProvider);
        Assert.Equal(updateRequest.IsCached, updatedStep.IsCached);
    }

    [Fact]
    public async Task DeleteAgentStepCommand_ShouldDeleteAgentStepFromDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var agentStepRepository = scope.ServiceProvider.GetRequiredService<IAgentStepRepository>();

        var stepToDelete = new AgentStepEntity { Name = "Step to Delete", Order = 1, Instruction = "Instr to Delete", ModelProvider = AiAgent.Api.Domain.Chat.Enums.AiProvider.OpenAi };
        await agentStepRepository.InsertAsync(stepToDelete);

        var command = new DeleteAgentStepCommand { Id = stepToDelete.Id };

        // Act
        await mediator.SendAsync(command);

        // Assert
        var deletedStep = await agentStepRepository.GetByIdAsync(stepToDelete.Id);
        Assert.Null(deletedStep);
    }

    [Fact]
    public async Task GetAllKnowledgeQuery_ShouldReturnAllKnowledgeFromDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var knowledgeRepository = scope.ServiceProvider.GetRequiredService<IKnowledgeRepository>();

        var knowledge1 = new KnowledgeEntity { Key = "Key1", Module = "Mod1", SourceType = AiAgent.Api.Domain.Knowledge.Enums.KnowledgeSourceType.Inline };
        var knowledge2 = new KnowledgeEntity { Key = "Key2", Module = "Mod2", SourceType = AiAgent.Api.Domain.Knowledge.Enums.KnowledgeSourceType.BlobUrl };
        await knowledgeRepository.InsertAsync(knowledge1);
        await knowledgeRepository.InsertAsync(knowledge2);

        var query = new GetAllKnowledgeQuery();

        // Act
        var result = await mediator.QueryAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, k => k.Key == knowledge1.Key);
        Assert.Contains(result, k => k.Key == knowledge2.Key);
    }

    [Fact]
    public async Task UpdateKnowledgeCommand_ShouldUpdateKnowledgeInDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var knowledgeRepository = scope.ServiceProvider.GetRequiredService<IKnowledgeRepository>();

        var originalKnowledge = new KnowledgeEntity { Key = "OriginalKey", Module = "OriginalMod", SourceType = AiAgent.Api.Domain.Knowledge.Enums.KnowledgeSourceType.Inline };
        await knowledgeRepository.InsertAsync(originalKnowledge);

        var updateRequest = new UpdateKnowledgeRequest
        {
            Key = "UpdatedKey",
            Module = "UpdatedMod",
            SourceType = AiAgent.Api.Domain.Knowledge.Enums.KnowledgeSourceType.BlobUrl,
            BlobUrl = "http://updated.blob.url"
        };
        var command = new UpdateKnowledgeCommand { Id = originalKnowledge.Id, Request = updateRequest };

        // Act
        await mediator.SendAsync(command);

        // Assert
        var updatedKnowledge = await knowledgeRepository.GetByIdAsync(originalKnowledge.Id);
        Assert.NotNull(updatedKnowledge);
        Assert.Equal(updateRequest.Key, updatedKnowledge.Key);
        Assert.Equal(updateRequest.Module, updatedKnowledge.Module);
        Assert.Equal(updateRequest.SourceType, updatedKnowledge.SourceType);
        Assert.Equal(updateRequest.BlobUrl, updatedKnowledge.BlobUrl);
    }

    [Fact]
    public async Task DeleteKnowledgeCommand_ShouldDeleteKnowledgeFromDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var knowledgeRepository = scope.ServiceProvider.GetRequiredService<IKnowledgeRepository>();

        var knowledgeToDelete = new KnowledgeEntity { Key = "KeyToDelete", Module = "ModToDelete", SourceType = AiAgent.Api.Domain.Knowledge.Enums.KnowledgeSourceType.Inline };
        await knowledgeRepository.InsertAsync(knowledgeToDelete);

        var command = new DeleteKnowledgeCommand { Id = knowledgeToDelete.Id };

        // Act
        await mediator.SendAsync(command);

        // Assert
        var deletedKnowledge = await knowledgeRepository.GetByIdAsync(knowledgeToDelete.Id);
        Assert.Null(deletedKnowledge);
    }

    [Fact]
    public async Task RegisterBlobCommand_ShouldRegisterBlobKnowledgeInDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var knowledgeRepository = scope.ServiceProvider.GetRequiredService<IKnowledgeRepository>();

        var request = new RegisterBlobRequest
        {
            Key = "BlobKey",
            Module = "BlobModule",
            BlobUrl = "http://test.blob.url"
        };
        var command = new RegisterBlobCommand { Request = request };

        // Act
        await mediator.SendAsync(command);

        // Assert
        var registeredKnowledge = await knowledgeRepository.GetByKeyAndModuleAsync(request.Key, request.Module);
        Assert.NotNull(registeredKnowledge);
        Assert.Equal(request.Key, registeredKnowledge.Key);
        Assert.Equal(request.Module, registeredKnowledge.Module);
        Assert.Equal(request.BlobUrl, registeredKnowledge.BlobUrl);
        Assert.Equal(AiAgent.Api.Domain.Knowledge.Enums.KnowledgeSourceType.BlobUrl, registeredKnowledge.SourceType);
    }

    [Fact]
    public async Task UploadJsonlCommand_ShouldUploadKnowledgeDataInDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var knowledgeRepository = scope.ServiceProvider.GetRequiredService<IKnowledgeRepository>();

        var jsonlContent = "{\"Key\":\"JsonlKey1\",\"Module\":\"JsonlModule\",\"SourceType\":0,\"Items\":[{\"Key\":\"fact1\",\"Value\":\"value1\"}]}";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonlContent));
        var file = new FormFile(stream, 0, stream.Length, "file", "test.jsonl");

        var command = new UploadJsonlCommand { File = file, Module = "JsonlModule" };

        // Act
        await mediator.SendAsync(command);

        // Assert
        var uploadedKnowledge = await knowledgeRepository.GetByKeyAndModuleAsync("JsonlKey1", "JsonlModule");
        Assert.NotNull(uploadedKnowledge);
        Assert.Equal("JsonlKey1", uploadedKnowledge.Key);
        Assert.Equal("JsonlModule", uploadedKnowledge.Module);
        Assert.Equal(AiAgent.Api.Domain.Knowledge.Enums.KnowledgeSourceType.Inline, uploadedKnowledge.SourceType);
        Assert.Single(uploadedKnowledge.Items);
        Assert.Equal("fact1", uploadedKnowledge.Items[0].Key);
    }

    [Fact]
    public async Task GetAllStepCachesQuery_ShouldReturnAllStepCachesFromDatabase()
    {
        // Arrange
        await factory.ClearDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var stepCacheRepository = scope.ServiceProvider.GetRequiredService<IStepCacheRepository>();

        var cache1 = new StepCacheEntity { AgentStepId = Guid.NewGuid(), Query = "q1", Value = "v1" };
        var cache2 = new StepCacheEntity { AgentStepId = Guid.NewGuid(), Query = "q2", Value = "v2" };
        await stepCacheRepository.InsertAsync(cache1);
        await stepCacheRepository.InsertAsync(cache2);

        var query = new GetAllStepCachesQuery();

        // Act
        var result = await mediator.QueryAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Query == cache1.Query);
        Assert.Contains(result, c => c.Query == cache2.Query);
    }
}