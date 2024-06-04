namespace PCA.Core.Entities;

public class Subscription: BaseEntity
{
    [JsonPropertyName("subscription")]
    public bool IsEnabled { get; set; }
    
    [JsonPropertyName("entityObjectType")]
    public string? EntityObjectType { get; set; }
    
    [JsonPropertyName("eventTypes")]
    public List<string> EventTypes { get; set; } = new();
    
    [JsonPropertyName("filters")]
    public string? Filters { get; set; }
}