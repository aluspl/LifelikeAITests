using System.Text.Json;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Interfaces;

namespace AiAgent.Api.Domain.Knowledge.Services;

public class DataKnowledgeService : IDataKnowledgeService
{
    private readonly IKnowledgeRepository _knowledgeRepository;

    public DataKnowledgeService(IKnowledgeRepository knowledgeRepository)
    {
        _knowledgeRepository = knowledgeRepository;
    }

    public async Task<int> UploadKnowledgeDataAsync(Stream fileStream, string moduleString)
    {
        var count = 0;
        using var reader = new StreamReader(fileStream);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var knowledgeDto = JsonSerializer.Deserialize<JsonlDto>(line, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (knowledgeDto?.Contents != null)
            {
                var userContent = knowledgeDto.Contents.FirstOrDefault(c => c.Role == "user");
                var modelContent = knowledgeDto.Contents.FirstOrDefault(c => c.Role == "model");

                if (userContent?.Parts.FirstOrDefault()?.Text != null && modelContent?.Parts.FirstOrDefault()?.Text != null)
                {
                    var key = userContent.Parts.First().Text;
                    var value = modelContent.Parts.First().Text;

                    var entity = new KnowledgeEntity
                    {
                        Key = key,
                        Value = value,
                        Module = moduleString, // Use moduleString directly
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };

                    Console.WriteLine($"Upserting Key: {entity.Key}, Module: {entity.Module}");
                    await _knowledgeRepository.UpsertAsync(entity);
                    count++;
                }
            }
        }

        return count;
    }

    // A private DTO to represent a line in the jsonl file.
    private class JsonlDto
    {
        public List<ContentItem> Contents { get; set; } = new List<ContentItem>();
    }

    private class ContentItem
    {
        public string Role { get; set; } = string.Empty;
        public List<Part> Parts { get; set; } = new List<Part>();
    }

    private class Part
    {
        public string Text { get; set; } = string.Empty;
    }
}
