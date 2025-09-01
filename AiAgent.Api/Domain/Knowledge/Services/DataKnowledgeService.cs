using System.Text.Json;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Enums;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using AiAgent.Api.Domain.Knowledge.Models;

namespace AiAgent.Api.Domain.Knowledge.Services;

public class DataKnowledgeService(IKnowledgeRepository knowledgeRepository) : IDataKnowledgeService
{
    public async Task<int> UploadKnowledgeDataAsync(Stream fileStream, string moduleString)
    {
        var count = 0;
        using var reader = new StreamReader(fileStream);

        var knowledgeItems = new List<KnowledgeItem>();
        while (await reader.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var jsonlDto = JsonSerializer.Deserialize<JsonlDto>(line, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (jsonlDto?.Contents == null) continue;
            var userContent = jsonlDto.Contents.FirstOrDefault(c => c.Role == "user");
            var modelContent = jsonlDto.Contents.FirstOrDefault(c => c.Role == "model");

            if (userContent?.Parts.FirstOrDefault()?.Text == null ||
                modelContent?.Parts.FirstOrDefault()?.Text == null) continue;
            knowledgeItems.Add(new KnowledgeItem
            {
                Key = userContent.Parts.First().Text,
                Value = modelContent.Parts.First().Text
            });
            count++;
        }

        if (!knowledgeItems.Any())
        {
            return count;
        }
        var knowledgeEntity = new KnowledgeEntity
        {
            Key = Guid.NewGuid().ToString(), // Generate a unique key for this collection of items
            Module = moduleString,
            SourceType = KnowledgeSourceType.Inline,
            Items = knowledgeItems,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };
        await knowledgeRepository.UpsertAsync(knowledgeEntity);

        return count;
    }

    public async Task UploadBlobKnowledgeAsync(string key, string module, string blobUrl)
    {
        var knowledgeEntity = new KnowledgeEntity
        {
            Key = key, // Use Key as the logical key
            Module = module,
            SourceType = KnowledgeSourceType.BlobUrl,
            BlobUrl = blobUrl,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };
        await knowledgeRepository.UpsertAsync(knowledgeEntity);
    }

    public async Task PopulateKnowledgeFromDictionaryAsync(string key, string module, Dictionary<string, string> data)
    {
        var knowledgeItems = data.Select(kvp => new KnowledgeItem { Key = kvp.Key, Value = kvp.Value }).ToList();

        var knowledgeEntity = new KnowledgeEntity
        {
            Key = key, // Use provided key
            Module = module,
            SourceType = KnowledgeSourceType.Inline,
            Items = knowledgeItems,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };
        await knowledgeRepository.UpsertAsync(knowledgeEntity);
    }

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
