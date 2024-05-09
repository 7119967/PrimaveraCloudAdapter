namespace PCA.Core.Entities;

public class Event: BaseEntity
{
    public string? MessageType { get; set; }
    public string? Code { get; set; }
    public string? EventType { get; set; }
    public int? PrimaryKey { get; set; }
    public int? WorkspaceId { get; set; }
    public string? WorkspaceCode { get; set; }
    public string? EntityObjectType { get; set; }
    public string? UpdatedFields { get; set; }
}
