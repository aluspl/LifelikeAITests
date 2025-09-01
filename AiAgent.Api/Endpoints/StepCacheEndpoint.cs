using AiAgent.Api.Domain.StepCache.Interfaces;

namespace AiAgent.Api.Endpoints;

public static class StepCacheEndpoint
{
    public static WebApplication MapStepCacheEndpoints(this WebApplication app)
    {
        app.MapGet("/step-cache/{agentId:guid}", async (Guid agentId, IStepCacheService service) =>
        {
            var caches = await service.GetStepCaches(agentId);
            return Results.Ok(caches);
        });

        app.MapGet("/step-cache/all", async (IStepCacheService services) =>
        {
            var result = await services.GetAllStepCaches();
            return Results.Ok(result);
        });

        return app;
    }
}