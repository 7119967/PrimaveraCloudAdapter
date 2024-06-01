namespace PCA.Core.Entities;

public class Resource: BaseEntity
{
    public virtual Transaction? Transaction { get; set; }
    public long TransactionId { get; set; }
    public string? Json { get; set; }
}