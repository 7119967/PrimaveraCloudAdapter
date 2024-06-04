namespace PCA.Core.Entities;

public class Transaction: BaseEntity
{
    [JsonIgnore] 
    public virtual EventNotification? Event { get; set; }
    public long EventId { get; set; }
    public string? EventType { get; set; }
    public DateTimeOffset EventTimeStamp { get; set; }
    public string? EventDetails { get; set; }
    public string? SystemOrigin { get; set; }
    public string? InitiatingUser { get; set; }

    [JsonIgnore] 
    public virtual ICollection<Activity>? Activities { get; set; }
    [JsonIgnore]
    public virtual ICollection<ActivityRelationship>? ActivityRelationships { get; set; }
    [JsonIgnore]
    public virtual ICollection<CalendarChange>? CalendarChanges { get; set; }
    [JsonIgnore]
    public virtual ICollection<Project>? Projects { get; set; }
    [JsonIgnore]
    public virtual ICollection<ProjectBudget>? ProjectBudgets { get; set; }
    [JsonIgnore]
    public virtual ICollection<Resource>? Resources { get; set; }
    [JsonIgnore]
    public virtual ICollection<ResourceAssignment>? ResourceAssignments { get; set; }
    [JsonIgnore]
    public virtual ICollection<Wbs>? Wbses { get; set; }
}