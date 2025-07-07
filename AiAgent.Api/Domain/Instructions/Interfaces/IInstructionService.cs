using AiAgent.Api.Domain.Instructions.Models;

namespace AiAgent.Api.Domain.Instructions.Interfaces;

public interface IInstructionService
{
    Task<string> GetInstructionAsync(string module);
    Task<string> UpsertInstructionAsync(InstructionRequest request);
}
