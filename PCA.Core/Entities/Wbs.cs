namespace PCA.Core.Entities;

public class Wbs: BaseEntity
{
    [JsonIgnore]
    public virtual Transaction? Transaction { get; set; }
    public long TransactionId { get; set; }
    public string? Json { get; set; }
}