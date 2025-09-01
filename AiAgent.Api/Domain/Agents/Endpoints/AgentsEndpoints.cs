using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Agents.Orchestration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AiAgent.Api.Domain.Agents.Queries;
using AiAgent.Api.Domain.Agents.Commands;
using AiAgent.Api.Domain.Agents.Models;
using AiAgent.Api.Infrastructure.CQRS.Interfaces;

namespace AiAgent.Api.Domain.Agents.Endpoints;

public static class AgentsEndpoints
{
    public static void MapAgentsEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/agents");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var query = new GetAllAgentsQuery();
            var agents = await mediator.QueryAsync(query);
            return Results.Ok(agents);
        });

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var query = new GetAgentByIdQuery { Id = id };
            var agent = await mediator.QueryAsync(query);
            if (agent == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(agent);
        })
        .WithName("GetAgentById");

        group.MapPost("/", async (CreateAgentRequest request, IMediator mediator) =>
        {
            var command = new CreateAgentCommand { Request = request };
            await mediator.SendAsync(command);
            // Assuming the command handler sets the ID on the entity and we can retrieve it
            // This might require returning the created entity from the command handler
            // For now, we'll return a generic CreatedAtRoute
            return Results.CreatedAtRoute("GetAgentById", new { id = Guid.NewGuid() }, request); // Placeholder ID
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateAgentRequest request, IMediator mediator) =>
        {
            var command = new UpdateAgentCommand { Id = id, Name = request.Name, Description = request.Description };
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
            var command = new DeleteAgentCommand { Id = id };
            await mediator.SendAsync(command);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/execute", async (Guid id, [FromBody] List<Dictionary<string, string>> parameters, IAgentOrchestratorService agentOrchestratorService) =>
        {
            try
            {
                var initialInputDict = new Dictionary<string, string>();
                foreach (var entry in parameters.SelectMany(paramDict => paramDict))
                {
                    initialInputDict[entry.Key] = entry.Value;
                }
                var initialInputJson = System.Text.Json.JsonSerializer.Serialize(initialInputDict);

                var result = await agentOrchestratorService.ExecuteAgent(id, initialInputJson);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }
}
