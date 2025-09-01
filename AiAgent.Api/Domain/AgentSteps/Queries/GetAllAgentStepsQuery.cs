using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.AgentSteps.Queries;

public class GetAllAgentStepsQuery : IQuery<IEnumerable<AgentStepEntity>> { }