namespace PCA.Infrastructure.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceScope? _scope;
    private readonly IServiceCollection _services;
    private readonly Dictionary<string, Type> _entityObjectTypeToApiClientTypeMap = new()
    {
        { "ApiEntityWBS", typeof(ApiClientWbs) },
        { "ApiEntityProject", typeof(ApiClientProject) },
        { "ApiEntityActivity", typeof(ApiClientActivity) },
        { "ApiEntityResource", typeof(ApiClientResource) },
        { "ApiEntityRelationship", typeof(ApiClientRelationship) },
        { "ApiEntityProjectBudget", typeof(ApiClientProjectBudget) },
        { "ApiEntityResourceRoleAssignment", typeof(ApiClientAssignment) },
    };

    public MessageProcessor(IServiceCollection services)
    {
        _services = services;
        var serviceProvider = services.BuildServiceProvider(true);
        _scope = serviceProvider.CreateScope();
        _unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }
    
    public Task ProcessMessageAsync(string json)
    {
        try
        {
            if (json is null) return Task.CompletedTask;
            var message = JsonConvert.DeserializeObject(json);
            Console.WriteLine("New message received from Primavera Cloud is:");
            Console.WriteLine(JsonConvert.SerializeObject(message, Formatting.Indented));
            var obj = JsonConvert.DeserializeObject<ApiEntitySubscriptionView>(json);

            if (_entityObjectTypeToApiClientTypeMap.TryGetValue(obj!.EntityObjectType, out var apiClientType))
            {
                var method = typeof(MessageProcessor)
                    .GetMethod("ProcessApiEntity", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(apiClientType);
                method.Invoke(this, new object[] { obj, message! });
            }
            else
            {
                Console.WriteLine($"Unknown EntityObjectType: {obj.EntityObjectType}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing the message: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    private EventNotification? GetEventNotification(ApiEntitySubscriptionView obj)
    {
        var existEventNotification = _unitOfWork.EventNotificationRepository.GetNoTracking()
            .FirstOrDefault(e => e.EntityObjectType!.Contains(obj.EntityObjectType) && e.MessageType == "SUCCESS");

        return existEventNotification;
    }

    private Subscription? GetSubscription(ApiEntitySubscriptionView obj)
    {
        var subscription = _unitOfWork.SubscriptionRepository.GetNoTracking()
            .FirstOrDefault(e => e.EntityObjectType!.Contains(obj.EntityObjectType));

        if (subscription is null && obj.EntityObjectType == "ApiEntityProjectBudget")
        {
            subscription = _unitOfWork.SubscriptionRepository.GetNoTracking()
                .FirstOrDefault(e => e.EntityObjectType!.Contains("ApiEntityProject"));
        }

        return subscription;
    }

    private EventNotification SaveEventNotification(ApiEntitySubscriptionView obj)
    {
        var subscription = GetSubscription(obj);

        var eventTypeList = new List<string>
        {
            obj.EntityEventType
        };

        var eventNotification = new EventNotification
        {
            // Subscription = subscription!,
            SubscriptionId = subscription!.Id,
            Message = obj.MessageContent,
            MessageType = obj.MessageType,
            IsEnabled = subscription.IsEnabled,
            EntityObjectType = obj.EntityObjectType,
            EntityEventType = eventTypeList
        };

        var existEventNotification = GetEventNotification(obj);

        if (existEventNotification != null) 
        {
            if (existEventNotification.EntityEventType.Contains(obj.EntityEventType))
            {
                return existEventNotification;
            }

            existEventNotification.EntityEventType.Add(obj.EntityEventType);
            var updated = _unitOfWork.EventNotificationRepository.Update(existEventNotification, new CancellationToken()).Result;
            return (updated as InsertedEvent<EventNotification>)?.Object!; ; 
        }

        var entity = _unitOfWork.EventNotificationRepository
            .Insert(eventNotification, new CancellationToken()).Result;

        return (entity as InsertedEvent<EventNotification>)?.Object!;
    }

    private void SaveTransaction(EventNotification eventNotification, ApiEntitySubscriptionView obj, dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        var entity = new Transaction
        {
            EventId = eventNotification.Id,
            EventType = obj.EntityObjectType,
            EventTimeStamp = DateTimeOffset.UtcNow,
            EventDetails = eventDetails,
            SystemOrigin = "primavera-ca1.oraclecloud.com",
            InitiatingUser = "vurbanovich@sarsystems.com"
        };
        _unitOfWork.TransactionRepository.Insert(entity, new CancellationToken());
    }

    private void CallApiClient<T>(EventNotification eventNotification, dynamic message) where T : IHttpClientStrategy<HttpResponseMessage>
    {
        var apiClientEntity = (T)Activator.CreateInstance(typeof(T), _services!)!;
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientEntity!);
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        apiHttpClient.ExecuteRequests(eventNotification, json).WaitAsync(new CancellationToken());
    }
    
/// <summary>
/// This method invokes by ProcessMessageAsync method
/// </summary>
/// <param name="obj"></param>
/// <param name="message"></param>
/// <typeparam name="T"></typeparam>
    private void ProcessApiEntity<T>(ApiEntitySubscriptionView obj, dynamic message) where T : IHttpClientStrategy<HttpResponseMessage>
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                var eventNotification = GetEventNotification(obj);
                SaveTransaction(eventNotification, obj, message);
                CallApiClient<T>(eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                SaveEventNotification(obj);
                break;
        }
    }
}