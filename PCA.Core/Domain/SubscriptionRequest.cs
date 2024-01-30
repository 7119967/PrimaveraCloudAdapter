namespace PCA.Core.Domain;

public class SubscriptionRequest
{
    public bool Subscription { get; set; }
    public string EntityObjectType { get; set; }
    public List<string> EventTypes { get; set; }
    public string Filters { get; set; }

    public SubscriptionRequest(bool subscription, string entityObjectType, List<string> eventTypes, string filters)
    {
        Subscription = subscription;
        EntityObjectType = entityObjectType;
        EventTypes = eventTypes;
        Filters = filters;
    }
}