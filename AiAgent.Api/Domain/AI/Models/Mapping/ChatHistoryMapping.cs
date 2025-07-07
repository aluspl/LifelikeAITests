using AiAgent.Api.Domain.AI.Models.Response;
using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.AI.Models.Mapping;

public static class ChatHistoryMappingExtensions
{
    public static ChatHistoryResponse ToResponse(this ChatHistoryEntity entity)
    {
        if (entity == null)
            return null;

        return new ChatHistoryResponse
        {
            Id = entity.Id,
            Query = entity.Query,
            Response = entity.Response,
            Model = entity.Model,
            Created = entity.Created,
            Role = entity.Role
        };
    }

    public static IEnumerable<ChatHistoryResponse> ToResponse(this IEnumerable<ChatHistoryEntity> entities)
    {
        return entities?.Select(e => e.ToResponse());
    }
}