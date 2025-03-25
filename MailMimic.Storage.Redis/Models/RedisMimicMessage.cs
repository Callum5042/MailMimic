using MailMimic.Models;
using Redis.OM.Modeling;
using System.Text.Json;

namespace MailMimic.Storage.Redis.Models;

[Document(IndexName = "mimic-messages", StorageType = StorageType.Json)]
public class RedisMimicMessage
{
    [RedisIdField]
    public string? Id { get; set; }

    [Indexed]
    public string? Content { get; set; }

    // Convert from application model
    public static RedisMimicMessage FromDomain(MimicMessageEntity message)
    {
        var json = JsonSerializer.Serialize(message);

        return new RedisMimicMessage
        {
            Id = message.Id.ToString(),
            Content = json,
        };
    }

    // Convert back to application model
    public MimicMessageEntity ToDomain()
    {
        if (string.IsNullOrEmpty(Content))
        {
            return null!;
        }

        var message = JsonSerializer.Deserialize<MimicMessageEntity>(Content);
        return message!;
    }
}