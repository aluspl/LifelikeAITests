using Microsoft.AspNetCore.Mvc;
using AiAgent.Api.Domain.Knowledge.Queries;
using AiAgent.Api.Domain.Knowledge.Commands;
using AiAgent.Api.Domain.Knowledge.Models;
using AiAgent.Api.Infrastructure.CQRS.Interfaces;

namespace AiAgent.Api.Domain.Knowledge.Endpoints;

public static class KnowledgeEndpoints
{
    public static void MapKnowledgeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/knowledge");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var query = new GetAllKnowledgeQuery();
            var knowledge = await mediator.QueryAsync(query);
            return Results.Ok(knowledge);
        });

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var query = new GetKnowledgeByIdQuery { Id = id };
            var knowledge = await mediator.QueryAsync(query);
            return knowledge == null ? Results.NotFound() : Results.Ok(knowledge);
        })
        .WithName("GetKnowledgeById");

        group.MapPost("/", async (CreateKnowledgeRequest request, IMediator mediator) =>
        {
            var command = new CreateKnowledgeCommand { Request = request };
            await mediator.SendAsync(command);
            return Results.Ok();
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateKnowledgeRequest request, IMediator mediator) =>
        {
            var command = new UpdateKnowledgeCommand { Id = id, Request = request };
            try
            {
                await mediator.SendAsync(command);
                return Results.NoContent();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var command = new DeleteKnowledgeCommand { Id = id };
            await mediator.SendAsync(command);
            return Results.NoContent();
        });

        group.MapPost("/upload-jsonl", async (IFormFile file, [FromQuery] string module, IMediator mediator) =>
        {
            var command = new UploadJsonlCommand { File = file, Module = module };
            try
            {
                await mediator.SendAsync(command);
                return Results.Ok("File uploaded and processed successfully.");
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        group.MapPost("/register-blob", async ([FromBody] RegisterBlobRequest request, IMediator mediator) =>
        {
            var command = new RegisterBlobCommand { Request = request };
            try
            {
                await mediator.SendAsync(command);
                return Results.Ok($"Successfully registered blob knowledge for key '{request.Key}' in module '{request.Module}'.");
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }
}



