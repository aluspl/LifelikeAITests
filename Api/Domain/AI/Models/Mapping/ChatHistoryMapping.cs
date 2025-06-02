using Api.Domain.AI.Models.Response;
using Api.Domain.Database.Entites;

namespace Api.Domain.AI.Extensions;

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