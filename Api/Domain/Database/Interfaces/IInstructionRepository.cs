using Api.Domain.Database.Entites;

namespace Api.Domain.Database.Interfaces;

public interface IInstructionRepository : IRepository<InstructionEntity>
{
    Task<InstructionEntity> GetByModuleAsync(string module);
}
