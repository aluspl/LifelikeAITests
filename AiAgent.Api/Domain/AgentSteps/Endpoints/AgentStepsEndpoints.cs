using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AiAgent.Api.Domain.AgentSteps.Queries;
using AiAgent.Api.Domain.AgentSteps.Commands;
using AiAgent.Api.Domain.AgentSteps.Models;
using AiAgent.Api.Infrastructure.CQRS.Interfaces;

namespace AiAgent.Api.Domain.AgentSteps.Endpoints;

public static class AgentStepsEndpoints
{
    public static void MapAgentStepsEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/agent-steps");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var query = new GetAllAgentStepsQuery();
            var steps = await mediator.QueryAsync(query);
            return Results.Ok(steps);
        });

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var query = new GetAgentStepByIdQuery { Id = id };
            var step = await mediator.QueryAsync(query);
            if (step == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(step);
        })
        .WithName("GetAgentStepById");

        group.MapPost("/", async (CreateAgentStepRequest request, IMediator mediator) =>
        {
            var command = new CreateAgentStepCommand { Request = request };
            await mediator.SendAsync(command);
            // Assuming the command handler sets the ID on the entity and we can retrieve it
            // This might require returning the created entity from the command handler
            // For now, we'll return a generic CreatedAtRoute
            return Results.CreatedAtRoute("GetAgentStepById", new { id = Guid.NewGuid() }, request); // Placeholder ID
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateAgentStepRequest request, IMediator mediator) =>
        {
            var command = new UpdateAgentStepCommand { Id = id, Request = request };
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
            var command = new DeleteAgentStepCommand { Id = id };
            await mediator.SendAsync(command);
            return Results.NoContent();
        });
    }
}
