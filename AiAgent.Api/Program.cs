using System.Runtime.CompilerServices;
using AiAgent.Api.Extensions;
using AiAgent.Api.Domain.Chat.Models;
using AiAgent.Api.Domain.Chat.Interfaces;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Instructions.Interfaces;
using AiAgent.Api.Domain.Instructions.Models;
using AiAgent.Api.Domain.Agents.KillTeam.Models;
using AiAgent.Api.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using AiAgent.Api.Auth;
using AiAgent.Api.Domain.Common.Enums;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using AiAgent.Api.Middleware;
using AiAgent.Api.Domain.AI.Interfaces;
using static AiAgent.Api.Domain.AI.Models.Mapping.ChatHistoryMappingExtensions;

[assembly: InternalsVisibleTo("AiAgent.Api.IntegrationTests")]

namespace AiAgent.Api;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.SetupServices(builder.Configuration);
        builder.Services.AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationOptions.DefaultScheme, options => { });
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            var showScalar = builder.Configuration.GetValue<bool>("ShowScalar");
            if (showScalar)
            {
                app.MapScalarApiReference();
            }
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGroup("/").RequireAuthorization();

        app.MapHealthChecks("/");

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

        app.MapPost("/agents/killteam/analyze", async ([FromBody] KillTeamAnalysisRequest request, IAiAnalysisService aiAnalysisService) =>
        {
            var response = await aiAnalysisService.AnalyzeKillTeamsAsync(request.Team1Name, request.Team2Name, request.Language);
            return Results.Ok(response);
        });

        app.MapPost("/knowledge/upload", async (HttpRequest request, IDataKnowledgeService dataKnowledgeService) =>
        {
            var moduleString = request.Query["module"];
            if (!request.HasFormContentType)
            {
                return Results.BadRequest("Request must be multipart/form-data.");
            }

            var form = await request.ReadFormAsync();
            var file = form.Files["file"]; // Assuming the file input name is "file"

            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("No file uploaded.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".jsonl")
            {
                return Results.BadRequest("Invalid file type. Please upload a .jsonl file.");
            }

            if (!Enum.TryParse(moduleString, true, out ModuleType module))
            {
                return Results.BadRequest($"Invalid module specified: {moduleString}. Valid modules are: {string.Join(", ", Enum.GetNames(typeof(ModuleType)))}");
            }

            // Pass the file stream directly
            var count = await dataKnowledgeService.UploadKnowledgeDataAsync(file.OpenReadStream(), moduleString);
            return Results.Ok($"Successfully seeded {count} knowledge entries for module {moduleString}.");
        }).Accepts<IFormFile>("multipart/form-data");

        app.MapGet("/knowledge/get/{key}/{module}", async (string key, string module, IKnowledgeRepository knowledgeRepository) =>
        {
            var entity = await knowledgeRepository.GetByKeyAndModuleAsync(key, module);
            if (entity == null)
            {
                return Results.NotFound($"Knowledge entry not found for key '{key}' and module '{module}'.");
            }
            return Results.Ok(entity);
        });

        app.MapGet("/knowledge/all/{module}", async (string module, IKnowledgeRepository knowledgeRepository) =>
        {
            var entities = await knowledgeRepository.GetAllByModuleAsync(module);

            if (!entities.Any())
            {
                return Results.NotFound($"No knowledge entries found for module '{module}'.");
            }
            return Results.Ok(entities);
        });

        // Seed initial API keys
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeederService>();
            await seeder.SeedApiKeysAsync();
        }

        app.Run();
    }
}
