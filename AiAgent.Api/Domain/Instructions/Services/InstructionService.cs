using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Instructions.Interfaces;
using AiAgent.Api.Domain.Instructions.Models;

namespace AiAgent.Api.Domain.Instructions.Services;

public class InstructionService(IInstructionRepository instructionRepository) : IInstructionService
{
    public async Task<string> GetInstructionAsync(string module)
    {
        var instruction = await instructionRepository.GetByModuleAsync(module);
        return instruction?.Content;
    }

    public async Task<string> UpsertInstructionAsync(InstructionRequest request)
    {
        var instruction = await instructionRepository.GetByModuleAsync(request.Module);
        if (instruction is null)
        {
            instruction = new InstructionEntity
            {
                Module = request.Module,
                Content = request.Content,
                Created = DateTime.UtcNow
            };
            await instructionRepository.InsertAsync(instruction);
            return $"Instruction for module '{request.Module}' created.";
        }
        else
        {
            instruction.Content = request.Content;
            instruction.Updated = DateTime.UtcNow;
            await instructionRepository.UpdateAsync(instruction);
            return $"Instruction for module '{request.Module}' updated.";
        }
    }
}
