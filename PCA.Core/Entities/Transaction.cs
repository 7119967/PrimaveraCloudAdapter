namespace PCA.Core.Entities;

public class Transaction: BaseEntity
{
    public virtual EventNotification? Event { get; set; }
    public long EventId { get; set; }
    public string? EventType { get; set; }
    public DateTimeOffset EventTimeStamp { get; set; }
    public string? EventDetails { get; set; }
    public string? SystemOrigin { get; set; }
    public string? InitiatingUser { get; set; }

    public virtual ICollection<Activity>? Activities { get; set; }
    public virtual ICollection<ActivityRelationship>? ActivityRelationships { get; set; }
    public virtual ICollection<CalendarChange>? CalendarChanges { get; set; }
    public virtual ICollection<Project>? Projects { get; set; }
    public virtual ICollection<ProjectBudget>? ProjectBudgets { get; set; }
    public virtual ICollection<Resource>? Resources { get; set; }
    public virtual ICollection<ResourceAssignment>? ResourceAssignments { get; set; }
    public virtual ICollection<Wbs>? Wbses { get; set; }
}