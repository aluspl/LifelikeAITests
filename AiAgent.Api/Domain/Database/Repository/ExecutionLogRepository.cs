using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using MongoDB.Driver;

namespace AiAgent.Api.Domain.Database.Repository
{
    public class ExecutionLogRepository(IMongoDatabase database)
        : Repository<ExecutionLogEntity>(database, "ExecutionLogs"), IExecutionLogRepository;
}
