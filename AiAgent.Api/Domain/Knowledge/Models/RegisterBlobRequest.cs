namespace AiAgent.Api.Domain.Knowledge.Models;

public class RegisterBlobRequest
{
    public string Key { get; set; }
    public string Module { get; set; }
    public string BlobUrl { get; set; }
}