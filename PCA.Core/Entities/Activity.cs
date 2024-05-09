namespace PCA.Core.Entities;

public class Activity: BaseEntity
{
    public virtual EventNotification Event { get; set; }
    public long EventId { get; set; }
    public string Json { get; set; }
}