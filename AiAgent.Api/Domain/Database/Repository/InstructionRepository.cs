using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
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

    public async Task<InstructionEntity> GetByKeyAsync(string key)
    {
        return await Collection.Find(x => x.Key == key).FirstOrDefaultAsync();
    }
}