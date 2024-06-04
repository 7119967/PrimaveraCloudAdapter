namespace PCA.Core.Models;

public class SubscriptionView
{
    [JsonPropertyName("subscription")]
    public bool IsEnabled { get; set; }
    
    [Required(ErrorMessage = "The entity object type is not specified")]
    [JsonPropertyName("entityObjectType")]
    public string? EntityObjectType { get; set; }
    
    [Required(ErrorMessage = "At least one event type should be specified")]
    [JsonPropertyName("eventTypes")]
    public List<string> EventTypes { get; set; } = new();
    
    [JsonPropertyName("filters")]
    public string? Filters { get; set; }
}