namespace PCA.Core.Models;

public class ApiEntitySubscriptionView
{
    [JsonPropertyName("messageType")]
    public string? MessageType { get; set; }

    [JsonPropertyName("subscription")]
    public bool Subscription { get; set; }

    [JsonPropertyName("entityObjectType")]
    public string? EntityObjectType { get; set; }

    [JsonPropertyName("entityEventType")]
    public string? EntityEventType { get; set; }

    [JsonPropertyName("message")]
    public string? MessageContent { get; set; }
}