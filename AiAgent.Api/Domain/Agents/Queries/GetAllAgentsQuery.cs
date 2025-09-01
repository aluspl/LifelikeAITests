using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Agents.Queries;

public class GetAllAgentsQuery : IQuery<IEnumerable<AgentEntity>> { }