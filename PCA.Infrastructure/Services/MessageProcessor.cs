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
            var obj = JsonConvert.DeserializeObject<ApiSubscriptionView>(json);
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

    private void ProcessApiEntityRelationship(ApiSubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                SaveActivityRelationship(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityResourceRoleAssignment(ApiSubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                SaveResourceAssignment(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityResource(ApiSubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                SaveResource(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":   
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityWbs(ApiSubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                SaveWbs(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityActivity(ApiSubscriptionView obj, object? message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                SaveActivity(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityProjectBudget(ApiSubscriptionView obj, dynamic message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                SaveProjectBudget(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }

    private void ProcessApiEntityProject(ApiSubscriptionView obj, dynamic message)
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                SaveTransaction(_eventNotification, obj, message);
                SaveProject(_eventNotification, message);
                break;
            case "SUCCESS":
            case "ERROR":
                _eventNotification = GetOrCreateEventNotification(obj);
                break;
        }
    }
    
    private EventNotification GetOrCreateEventNotification(ApiSubscriptionView obj)
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
    
    private void SaveTransaction(EventNotification eventNotification, ApiSubscriptionView obj, dynamic message)
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
    
    private void SaveProject(EventNotification eventNotification, dynamic message)
    {        
        var apiClientProject = new ApiClientProject(_services!);
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientProject);
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        apiHttpClient.ExecuteRequests(eventNotification, json).WaitAsync(new CancellationToken());
    }
    
    private void SaveProjectBudget(EventNotification eventNotification, dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        var entity = new ProjectBudget
        {
            EventId = eventNotification.Id,
            Json = eventDetails
        };
        _unitOfWork.ProjectBudgetRepository.Insert(entity, new CancellationToken());
    }
    
    private void SaveActivity(EventNotification eventNotification, dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        var entity = new Activity
        {
            EventId = eventNotification.Id,
            Json = eventDetails
        };
        _unitOfWork.ActivityRepository.Insert(entity, new CancellationToken());
    }
    
    private void SaveActivityRelationship(EventNotification eventNotification, dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        var entity = new ActivityRelationship
        {
            EventId = eventNotification.Id,
            Json = eventDetails
        };
        _unitOfWork.ActivityRelationshipRepository.Insert(entity, new CancellationToken());
    }
    
    private void SaveResource(EventNotification eventNotification, dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        var entity = new Resource
        {
            EventId = eventNotification.Id,
            Json = eventDetails
        };
        _unitOfWork.ResourceRepository.Insert(entity, new CancellationToken());
    }
    
    private void SaveResourceAssignment(EventNotification eventNotification, dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        var entity = new ResourceAssignment
        {
            EventId = eventNotification.Id,
            Json = eventDetails
        };
        _unitOfWork.ResourceAssignmentRepository.Insert(entity, new CancellationToken());
    }    
    
    private void SaveWbs(EventNotification eventNotification, dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        var entity = new Wbs
        {
            EventId = eventNotification.Id,
            Json = eventDetails
        };
        _unitOfWork.WbsRepository.Insert(entity, new CancellationToken());
    }
}