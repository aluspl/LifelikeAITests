using Microsoft.AspNetCore.Mvc;
using Api.Domain.AI.Extensions;
using Api.Domain.AI.Interfaces;
using Api.Domain.AI.Services;
using Api.Domain.Chat.Enums;
using Api.Domain.Chat.Interfaces;
using Api.Domain.Chat.Models;
using Api.Domain.Chat.Services;
using Api.Domain.Configuration;
using Api.Domain.Database.Interfaces;
using Api.Domain.Database.Repository;
using Api.Domain.Instructions.Interfaces;
using Api.Domain.Instructions.Models;
using Api.Domain.Instructions.Services;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Runtime.CompilerServices;
using MongoDB.Driver;

[assembly: InternalsVisibleTo("Api.IntegrationTests")]

namespace Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("AI:OpenAI"));
        builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("AI:Gemini"));
        builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoDB"));

        builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoSettings>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        builder.Services.AddKeyedTransient<IAiService, OpenAiService>(nameof(AiProvider.OpenAi).ToLower());
        builder.Services.AddKeyedTransient<IAiService, GeminiService>(nameof(AiProvider.Gemini).ToLower());

        builder.Services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new ChatHistoryRepository(database);
        });
        builder.Services.AddScoped<IInstructionRepository, InstructionRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new InstructionRepository(database);
        });
        builder.Services.AddScoped<IChatService, ChatService>(provider =>
        {
            Func<string, IAiService> aiServiceFactory = key => provider.GetRequiredKeyedService<IAiService>(key);
            return new ChatService(aiServiceFactory, provider.GetRequiredService<IChatHistoryRepository>(), provider.GetRequiredService<IInstructionService>());
        });
        builder.Services.AddScoped<IInstructionService, InstructionService>();

        // Add Swagger services
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatGPT Challenge API", Version = "v1" });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatGPT Challenge API v1");
            });
        }

        app.MapPost("/chat", async ([FromBody] ChatRequest request, IChatService chatService) =>
        {
            var response = await chatService.ProcessAsync(request);
            return Results.Ok(response);
        });

        app.MapGet("/chat/history", async (IChatHistoryRepository repository) =>
        {
            var history = await repository.GetChatHistoryAsync();
            return history.ToResponse();
        });

        app.MapGet("/instructions/{module}", async (string module, IInstructionService instructionService) =>
        {
            var instruction = await instructionService.GetInstructionAsync(module);
            return Results.Ok(instruction);
        });

        app.MapPost("/instructions", async ([FromBody] InstructionRequest request, IInstructionService instructionService) =>
        {
            var result = await instructionService.UpsertInstructionAsync(request);
            return Results.Ok(result);
        });

        app.Run();
    }
}
