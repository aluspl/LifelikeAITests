namespace AiAgent.Api.Domain.AI.Models.Response;

public class ChatHistoryResponse
{
    public string Id { get; set; }
    public string Query { get; set; }
    public string Response { get; set; }
    public string Model { get; set; }
    public DateTime Created { get; set; }
    public string Role { get; set; }
}
