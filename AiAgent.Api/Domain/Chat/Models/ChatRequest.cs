using AiAgent.Api.Domain.Chat.Enums;

namespace AiAgent.Api.Domain.Chat.Models;

public class ChatRequest
{
    public string UserMessage { get; set; }
    public string Instructions { get; set; }
    public AiProvider Provider { get; set; }
}
