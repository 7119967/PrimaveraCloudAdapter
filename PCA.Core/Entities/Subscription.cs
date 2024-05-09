namespace PCA.Core.Entities;

public class Subscription: BaseEntity
{
    [JsonProperty("subscription")]
    public bool IsEnabled { get; set; }
    
    [JsonProperty("entityObjectType")]
    public string? EntityObjectType { get; set; }
    
    [JsonProperty("eventTypes")]
    public List<string> EventTypes { get; set; } = new();
    
    [JsonProperty("filters")]
    public string? Filters { get; set; }
}