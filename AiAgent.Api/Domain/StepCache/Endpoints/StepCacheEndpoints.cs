using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.StepCache.Queries;
using AiAgent.Api.Domain.StepCache.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AiAgent.Api.Domain.StepCache.Endpoints;

public static class StepCacheEndpoints
{
    public static void MapStepCacheEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/step-cache");

        group.MapGet("/{agentId:guid}", async (Guid agentId, IMediator mediator) =>
        {
            var query = new GetStepCachesByAgentIdQuery { AgentId = agentId };
            var caches = await mediator.QueryAsync(query);
            return Results.Ok(caches);
        });

        group.MapGet("/all", async (IMediator mediator) =>
        {
            var query = new GetAllStepCachesQuery();
            var result = await mediator.QueryAsync(query);
            return Results.Ok(result);
        });
    }
}