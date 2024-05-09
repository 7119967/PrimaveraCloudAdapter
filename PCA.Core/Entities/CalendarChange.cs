namespace PCA.Core.Entities;

public class CalendarChange: BaseEntity
{
    public virtual EventNotification Event { get; set; }
    public long EventId { get; set; }
    public string Json { get; set; }
}