namespace PCA.Core.Entities;

public class Project: BaseEntity
{
    [JsonIgnore]
    public virtual Transaction? Transaction { get; set; }
    public long TransactionId { get; set; }
    public string? Json { get; set; }
}