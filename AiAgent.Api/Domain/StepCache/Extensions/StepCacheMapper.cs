// filepath: AiAgent.Api/Domain/Database/Extensions/StepCacheMappingExtensions.cs

using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.StepCache.Models;

namespace AiAgent.Api.Domain.StepCache.Extensions
{
    public static class StepCacheMappingExtensions
    {
        public static StepCacheResponse ToResponse(this StepCacheEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new StepCacheResponse
            {
                Id = entity.Id,
                AgentStepId = entity.AgentStepId,
                Query = entity.Query,
                Value = entity.Value
            };
        }

        public static IEnumerable<StepCacheResponse> ToResponse(this IEnumerable<StepCacheEntity> entities)
        {
            return entities?.Select(e => e.ToResponse());
        }

        public static StepCacheEntity ToEntity(this StepCacheResponse response)
        {
            if (response == null)
            {
                return null;
            }

            return new StepCacheEntity
            {
                Id = response.Id,
                AgentStepId = response.AgentStepId,
                Query = response.Query,
                Value = response.Value
            };
        }
    }
}