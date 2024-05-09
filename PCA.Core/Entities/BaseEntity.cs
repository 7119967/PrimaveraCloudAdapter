namespace PCA.Core.Entities;

public interface IHasIdentity
{
    long Id { get; set; }
}

public interface IDatesCreateUpdate
{
    DateTimeOffset UpdatedAt { get; set; }

    DateTimeOffset CreatedAt { get; set; }
}
public abstract class BaseEntity : IHasIdentity, IDatesCreateUpdate
{
    public long Id { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;

    [System.Text.Json.Serialization.JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
}