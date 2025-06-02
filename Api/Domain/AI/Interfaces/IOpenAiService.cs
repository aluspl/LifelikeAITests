namespace Api.Domain.AI.Interfaces;

public interface IOpenAiService
{
    Task<string> Process(string userMessage);
}