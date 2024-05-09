namespace PCA.Core.Models;

public class ApiSubscriptionView
{
    [JsonProperty("messageType")]
    public string MessageType { get; set; }

    [JsonProperty("subscription")]
    public bool Subscription { get; set; }

    [JsonProperty("entityObjectType")]
    public string EntityObjectType { get; set; }

    [JsonProperty("entityEventType")]
    public string EntityEventType { get; set; }

    [JsonProperty("message")]
    public string MessageContent { get; set; }
}