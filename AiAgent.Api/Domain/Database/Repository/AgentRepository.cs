using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using MongoDB.Driver;

namespace AiAgent.Api.Domain.Database.Repository
{
    public class AgentRepository : Repository<AgentEntity>, IAgentRepository
    {
        public AgentRepository(IMongoDatabase database) : base(database, "Agents")
        {
        }
    }
}
