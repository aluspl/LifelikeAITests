using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using MongoDB.Driver;

namespace AiAgent.Api.Domain.Database.Repository
{
    public class AgentStepRepository : Repository<AgentStepEntity>, IAgentStepRepository
    {
        public AgentStepRepository(IMongoDatabase database) : base(database, "AgentSteps")
        {
        }
    }
}
