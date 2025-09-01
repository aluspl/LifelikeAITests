using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.AI.Services;
using AiAgent.Api.Domain.Common.Interfaces;
using AiAgent.Api.Domain.Configuration;
using AiAgent.Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json.Serialization;
using AiAgent.Api.Domain.Chat.Enums;
using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Infrastructure.CQRS.Services;

using AiAgent.Api.Domain.StepCache.Services;

namespace AiAgent.Api.Extensions;

public static class ServiceCollectionExtensions
{
    static ServiceCollectionExtensions()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    }

    public static IServiceCollection SetupServices(this IServiceCollection services)
    {
        services.AddMongo();
        services.AddAiProviders();

        // Register all services and handlers
        services.AddScoped<IMediator, Mediator>();
        services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<IService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        services.AddHealthChecks();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }

    public static IServiceCollection SetupConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<OpenAiSettings>(configuration.GetSection("AI:OpenAI"));
        services.Configure<GeminiSettings>(configuration.GetSection("AI:Gemini"));
        services.Configure<ApiKeySettings>(configuration.GetSection("DefaultApiKey"));
        services.Configure<MongoSettings>(configuration.GetSection("MongoDB"));

        return services;
    }

    private static IServiceCollection AddMongo(this IServiceCollection services)
    {

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

        return services;
    }

    public static IServiceCollection AddAiProviders(this IServiceCollection services)
    {
        services.AddKeyedTransient<IAiProvider, OpenAiProvider>(nameof(AiProvider.OpenAi).ToLower());
        services.AddKeyedTransient<IAiProvider, GeminiProvider>(nameof(AiProvider.Gemini).ToLower());

        return services;
    }
}