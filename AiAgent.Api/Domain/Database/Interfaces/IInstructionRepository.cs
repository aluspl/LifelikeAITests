using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Database.Interfaces;

public interface IInstructionRepository : IRepository<InstructionEntity>
{
    Task<InstructionEntity> GetByModuleAsync(string module);
    Task<InstructionEntity> GetByKeyAsync(string key);
}
