using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using MongoDB.Driver;

namespace AiAgent.Api.Domain.Database.Repository
{
    public class AgentStepRepository(IMongoDatabase database)
        : Repository<AgentStepEntity>(database, "AgentSteps"), IAgentStepRepository;
}
