namespace PCA.Infrastructure.Services;

public class Helper: IHelper
{
    public JObject GetJsonObject(SubscriptionView model)
    {
        return new JObject(
            new JProperty("subscription", model.IsEnabled),
            new JProperty("entityObjectType", model.EntityObjectType),
            new JProperty("eventTypes", JArray.FromObject(model.EventTypes)),
            new JProperty("filters", model.Filters)
        );
    }
}
