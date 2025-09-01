using AiAgent.Api.Domain.Agents.Endpoints;
using AiAgent.Api.Domain.AgentSteps.Endpoints;
using AiAgent.Api.Domain.Knowledge.Endpoints;
using AiAgent.Api.Domain.StepCache.Endpoints;

namespace AiAgent.Api.Extensions;

public static class EndpointMapper
{
    public static void MapDomainEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAgentsEndpoints();
        app.MapAgentStepsEndpoints();
        app.MapKnowledgeEndpoints();
        app.MapStepCacheEndpoints();
    }
}