using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Knowledge.Queries;

public class GetAllKnowledgeQuery : IQuery<IEnumerable<KnowledgeEntity>> { }