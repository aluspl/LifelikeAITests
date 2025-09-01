using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Queries;

namespace AiAgent.Api.Domain.Knowledge.QueryHandlers;

public class GetKnowledgeByIdQueryHandler(IKnowledgeRepository knowledgeRepository)
    : IQueryHandler<GetKnowledgeByIdQuery, KnowledgeEntity>
{
    public async Task<KnowledgeEntity> HandleAsync(GetKnowledgeByIdQuery query)
    {
        return await knowledgeRepository.GetByIdAsync(query.Id);
    }
}