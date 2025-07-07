using AiAgent.Api.Domain.Configuration;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AiAgent.Api.Domain.Database.Repository;

public class InstructionRepository : Repository<InstructionEntity>, IInstructionRepository
{
    public InstructionRepository(IMongoDatabase database) : base(database, "Instructions")
    {
    }

    public async Task<InstructionEntity> GetByModuleAsync(string module)
    {
        return await Collection.Find(x => x.Module == module).FirstOrDefaultAsync();
    }
}
