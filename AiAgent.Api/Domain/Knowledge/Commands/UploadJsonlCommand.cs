using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AiAgent.Api.Domain.Knowledge.Commands;

public class UploadJsonlCommand : ICommand
{
    public IFormFile File { get; set; }
    public string Module { get; set; }
}