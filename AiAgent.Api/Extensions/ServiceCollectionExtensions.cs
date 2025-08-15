using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.AI.Services;
using AiAgent.Api.Domain.Chat.Enums;



using AiAgent.Api.Domain.Common.Interfaces;
using AiAgent.Api.Domain.Common.Services;
using AiAgent.Api.Domain.Configuration;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Database.Repository;
using AiAgent.Api.Domain.Instructions.Interfaces;
using AiAgent.Api.Domain.Instructions.Services;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using AiAgent.Api.Domain.Knowledge.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.Json.Serialization;

namespace AiAgent.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<OpenAiSettings>(configuration.GetSection("AI:OpenAI"));
        services.Configure<GeminiSettings>(configuration.GetSection("AI:Gemini"));
        services.Configure<MongoSettings>(configuration.GetSection("MongoDB"));
        services.Configure<ApiKeySettings>(configuration.GetSection("DefaultApiKey"));

        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoSettings>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        services.AddKeyedTransient<IAiService, OpenAiService>(nameof(AiProvider.OpenAi).ToLower());
        services.AddKeyedTransient<IAiService, GeminiService>(nameof(AiProvider.Gemini).ToLower());

        
        services.AddScoped<IInstructionRepository, InstructionRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new InstructionRepository(database);
        });

        services.AddScoped<IKnowledgeRepository, KnowledgeRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new KnowledgeRepository(database);
        });
        services.AddScoped<IAgentRepository, AgentRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new AgentRepository(database);
        });
        services.AddScoped<IExecutionLogRepository, ExecutionLogRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new ExecutionLogRepository(database);
        });

        services.AddScoped<IAgentStepRepository, AgentStepRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new AgentStepRepository(database);
        });

        services.AddScoped<IApiKeyRepository, ApiKeyRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new ApiKeyRepository(database);
        });

        services.AddScoped<AiAgent.Api.Domain.Agents.Orchestration.IAgentOrchestratorService, AiAgent.Api.Domain.Agents.Orchestration.AgentOrchestratorService>();

        services.AddScoped<IDataSeederService>(provider =>
        {
            return new DataSeederService(
                provider.GetRequiredService<IKnowledgeRepository>(),
                provider.GetRequiredService<IApiKeyRepository>(),
                provider.GetRequiredService<IOptions<ApiKeySettings>>(),
                provider.GetRequiredService<IInstructionRepository>(),
                provider.GetRequiredService<IAgentRepository>(),
                provider.GetRequiredService<IAgentStepRepository>()
            );
        });
        services.AddScoped<IDataKnowledgeService, DataKnowledgeService>();
        
        services.AddScoped<IInstructionService, InstructionService>();

        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        services.AddHealthChecks();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}
