using System.Runtime.CompilerServices;
using AiAgent.Api.Extensions;
using AiAgent.Api.Domain.Chat.Models;
using AiAgent.Api.Domain.Chat.Interfaces;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Instructions.Interfaces;
using AiAgent.Api.Domain.Instructions.Models;
using Microsoft.AspNetCore.Mvc;
using AiAgent.Api.Domain.Database.Entites;
using Scalar.AspNetCore;
using static AiAgent.Api.Domain.AI.Models.Mapping.ChatHistoryMappingExtensions;

[assembly: InternalsVisibleTo("AiAgent.Api.IntegrationTests")]

namespace AiAgent.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.SetupServices(builder.Configuration);

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

        app.Run();
    }
}
