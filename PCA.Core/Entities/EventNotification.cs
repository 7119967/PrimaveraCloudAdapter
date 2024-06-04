namespace PCA.Core.Entities;

public class EventNotification: BaseEntity
{
    [JsonIgnore] 
    public virtual Subscription? Subscription { get; set; }
    public long SubscriptionId { get; set; }
    public bool IsEnabled { get; set; }
    public string? MessageType { get; set; }
    public string? Message { get; set; }
    public string? EntityObjectType { get; set; }
    public List<string> EntityEventType { get; set; } = new();
}