using Api.Domain.Configuration;
using Api.Domain.Database.Entites;
using Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Domain.Database.Repository;

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
