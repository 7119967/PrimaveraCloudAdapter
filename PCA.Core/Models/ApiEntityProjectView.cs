namespace PCA.Core.Models;

public class ApiEntityProjectView
{
    [JsonProperty("messageType")] 
    public string? MessageType { get; set; }

    [JsonProperty("code")] 
    public string? Code { get; set; }

    [JsonProperty("eventType")] 
    public string? EventType { get; set; }

    [JsonProperty("primaryKey")] 
    public int? PrimaryKey { get; set; }

    [JsonProperty("workspaceId")] 
    public int? WorkspaceId { get; set; }

    [JsonProperty("workspaceCode")] 
    public string? WorkspaceCode { get; set; }

    [JsonProperty("entityObjectType")] 
    public string? EntityObjectType { get; set; }

    [JsonProperty("updatedFields")] 
    public UpdatedFields? UpdatedFields { get; set; }
}

public class UpdatedFields
{
    [JsonProperty("noResourceActivityCount")]
    public int? NoResourceActivityCount { get; set; }

    [JsonProperty("totalActivityCount")] public int? TotalActivityCount { get; set; }

    [JsonProperty("noPredecessorActivityCount")]
    public int? NoPredecessorActivityCount { get; set; }

    [JsonProperty("openEndsActivityCount")]
    public int? OpenEndsActivityCount { get; set; }

    [JsonProperty("noSuccessorActivityCount")]
    public int? NoSuccessorActivityCount { get; set; }
}