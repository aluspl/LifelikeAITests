using AiAgent.Api.Domain.Instructions.Interfaces;
using AiAgent.Api.Domain.Instructions.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AiAgent.Api.Domain.Instructions
{
    public static class InstructionsEndpoints
    {
        public static WebApplication MapInstructionsEndpoints(this WebApplication app)
        {
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

            return app;
        }
    }
}
