using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Queries;

namespace AiAgent.Api.Domain.Knowledge.QueryHandlers;

public class GetAllKnowledgeQueryHandler(IKnowledgeRepository knowledgeRepository)
    : IQueryHandler<GetAllKnowledgeQuery, IEnumerable<KnowledgeEntity>>
{
    public async Task<IEnumerable<KnowledgeEntity>> HandleAsync(GetAllKnowledgeQuery query)
    {
        return await knowledgeRepository.GetAllAsync();
    }
}