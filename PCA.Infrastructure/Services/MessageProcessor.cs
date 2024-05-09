using PCA.Infrastructure.Services.HttpClients;

namespace PCA.Infrastructure.Services;

public class MessageProcessor : IMessageProcessor
{
    private EventNotification _eventNotification;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApiHttpClient _apiHttpClient;
    private readonly ApiClientProject _apiClientProject;
    private readonly IServiceScope? _scope;
    private readonly IServiceCollection _services;

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
            switch (obj!.EntityObjectType)
            {
                case "ApiEntityProject":
                    ProcessApiEntityProject(obj, message!);
                    break;
                case "ApiEntityProjectBudget":
                    ProcessApiEntityProjectBudget(obj, message!);
                    break;
                case "ApiEntityActivity":
                    ProcessApiEntityActivity(obj, message!);
                    break;                   
                case "ApiEntityWBS":
                    ProcessApiEntityWbs(obj, message!);
                    break;                  
                case "ApiEntityResource":
                    ProcessApiEntityResource(obj, message!);
                    break;
                case "ApiEntityResourceRoleAssignment":
                    ProcessApiEntityResourceRoleAssignment(obj, message!);
                    break;                 
                case "ApiEntityRelationship":
                    ProcessApiEntityRelationship(obj, message!);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing the message: {ex.Message}");
        }
        
        return Task.CompletedTask;
    }

    private void ProcessApiEntityRelationship(ApiEntitySubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                GetActivityRelationship(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityResourceRoleAssignment(ApiEntitySubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                GetResourceAssignment(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityResource(ApiEntitySubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                GetResource(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":   
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityWbs(ApiEntitySubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                GetWbs(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityActivity(ApiEntitySubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                GetActivity(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityProjectBudget(ApiEntitySubscriptionView obj, dynamic message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                GetProjectBudget(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityProject(ApiEntitySubscriptionView obj, dynamic message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                GetProject(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }
    
    private EventNotification GetOrCreateEventNotification(ApiEntitySubscriptionView obj)
    {
        var subscription = _unitOfWork.SubscriptionRepository.GetNoTracking()
            .FirstOrDefault(e => e.EntityObjectType!.Contains(obj.EntityObjectType));
        
        if (subscription is null && obj.EntityObjectType == "ApiEntityProjectBudget")
        {
            subscription = _unitOfWork.SubscriptionRepository.GetNoTracking()
                .FirstOrDefault(e => e.EntityObjectType!.Contains("ApiEntityProject"));
        }

        var eventTypeList = new List<string>();
        eventTypeList.Add(obj.EntityEventType);
        
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
            InitiatingUser= "vurbanovich@sarsystems.com"
        };
        _unitOfWork.TransactionRepository.Insert(entity, new CancellationToken());
    }
    
    private void GetProject(EventNotification eventNotification, dynamic message)
    {        
        var apiClientEntity = new ApiClientProject(_services!);
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientEntity);
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        apiHttpClient.ExecuteRequests(eventNotification, json).WaitAsync(new CancellationToken());
    }
    
    private void GetProjectBudget(EventNotification eventNotification, dynamic message)
    {
        var apiClientEntity = new ApiClientProjectBudget(_services!);
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientEntity);
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        apiHttpClient.ExecuteRequests(eventNotification, json).WaitAsync(new CancellationToken());
    }
    
    private void GetActivity(EventNotification eventNotification, dynamic message)
    {
        var apiClientEntity = new ApiClientActivity(_services!);
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientEntity);
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        apiHttpClient.ExecuteRequests(eventNotification, json).WaitAsync(new CancellationToken());
    }
    
    private void GetActivityRelationship(EventNotification eventNotification, dynamic message)
    {
        var apiClientEntity = new ApiClientRelationship(_services!);
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientEntity);
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        apiHttpClient.ExecuteRequests(eventNotification, json).WaitAsync(new CancellationToken());
    }
    
    private void GetResource(EventNotification eventNotification, dynamic message)
    {
        var apiClientEntity = new ApiClientResource(_services!);
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientEntity);
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        apiHttpClient.ExecuteRequests(eventNotification, json).WaitAsync(new CancellationToken());
    }
    
    private void GetResourceAssignment(EventNotification eventNotification, dynamic message)
    {
        var apiClientEntity = new ApiClientAssignment(_services!);
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientEntity);
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        apiHttpClient.ExecuteRequests(eventNotification, json).WaitAsync(new CancellationToken());
    }    
    
    private void GetWbs(EventNotification eventNotification, dynamic message)
    {
        var apiClientEntity = new ApiClientWbs(_services!);
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientEntity);
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        apiHttpClient.ExecuteRequests(eventNotification, json).WaitAsync(new CancellationToken());
    }
}