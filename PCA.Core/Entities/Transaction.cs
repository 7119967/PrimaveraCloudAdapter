namespace PCA.Core.Entities;

public class Transaction: BaseEntity
{
    public virtual EventNotification Event { get; set; }
    public long EventId { get; set; }
    public string EventType { get; set; }
    public DateTimeOffset EventTimeStamp { get; set; }
    public string EventDetails { get; set; }
    public string SystemOrigin { get; set; }
    public string InitiatingUser { get; set; }
}