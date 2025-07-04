namespace Api.Domain.AI.Interfaces;

public interface IAiService
{
    Task<string> ProcessAsync(string userMessage, string instructions);
}
