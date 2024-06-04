namespace PCA.Core.Models;

public class ApiEntityProjectView
{
    [JsonPropertyName("messageType")] 
    public string? MessageType { get; set; }

    [JsonPropertyName("code")] 
    public string? Code { get; set; }

    [JsonPropertyName("eventType")] 
    public string? EventType { get; set; }

    [JsonPropertyName("primaryKey")] 
    public int? PrimaryKey { get; set; }

    [JsonPropertyName("workspaceId")] 
    public int? WorkspaceId { get; set; }

    [JsonPropertyName("workspaceCode")] 
    public string? WorkspaceCode { get; set; }

    [JsonPropertyName("entityObjectType")] 
    public string? EntityObjectType { get; set; }

    [JsonPropertyName("updatedFields")] 
    public UpdatedFields? UpdatedFields { get; set; }
}

public class UpdatedFields
{
    [JsonPropertyName("noResourceActivityCount")]
    public int? NoResourceActivityCount { get; set; }

    [JsonPropertyName("totalActivityCount")] public int? TotalActivityCount { get; set; }

    [JsonPropertyName("noPredecessorActivityCount")]
    public int? NoPredecessorActivityCount { get; set; }

    [JsonPropertyName("openEndsActivityCount")]
    public int? OpenEndsActivityCount { get; set; }

    [JsonPropertyName("noSuccessorActivityCount")]
    public int? NoSuccessorActivityCount { get; set; }
}