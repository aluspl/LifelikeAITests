using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Knowledge.Models;

namespace AiAgent.Api.Domain.Knowledge.Commands;

public class RegisterBlobCommand : ICommand
{
    public RegisterBlobRequest Request { get; set; }
}