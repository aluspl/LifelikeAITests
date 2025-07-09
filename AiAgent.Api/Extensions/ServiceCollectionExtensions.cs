using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.AI.Services;
using AiAgent.Api.Domain.Agents.KillTeam.Interfaces;
using AiAgent.Api.Domain.Agents.KillTeam.Services;
using AiAgent.Api.Domain.Chat.Enums;
using AiAgent.Api.Domain.Chat.Interfaces;
using AiAgent.Api.Domain.Common.Interfaces;
using AiAgent.Api.Domain.Common.Services;
using AiAgent.Api.Domain.Chat.Services;
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

        services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new ChatHistoryRepository(database);
        });
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

        services.AddScoped<IApiKeyRepository, ApiKeyRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new ApiKeyRepository(database);
        });

        services.AddScoped<IKillTeamAnalysisService, KillTeamAnalysisService>();
        services.AddScoped<IDataSeederService, DataSeederService>();
        services.AddScoped<IDataKnowledgeService, DataKnowledgeService>();
        services.AddScoped<IAiAnalysisService, AiAnalysisService>();
        services.AddScoped<IChatService, ChatService>(provider =>
        {
            Func<string, IAiService> aiServiceFactory = key => provider.GetRequiredKeyedService<IAiService>(key);
            return new ChatService(aiServiceFactory, provider.GetRequiredService<IChatHistoryRepository>(), provider.GetRequiredService<IInstructionService>());
        });
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
