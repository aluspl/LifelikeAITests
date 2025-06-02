namespace Api.Domain.AI.Interfaces;

public interface IGeminiService
{
    Task<string> Process(string userMessage);
}