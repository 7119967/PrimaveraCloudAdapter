namespace PCA.Infrastructure.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceScope? _scope;
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MessageProcessor> _logger;
    
    private readonly Dictionary<string, Type> _entityObjectTypeToApiClientTypeMap = new()
    {
        { "ApiEntityWBS", typeof(ApiClientWbs) },
        { "ApiEntityProject", typeof(ApiClientProject) },
        { "ApiEntityActivity", typeof(ApiClientActivity) },
        { "ApiEntityResource", typeof(ApiClientResource) },
        { "ApiEntityRelationship", typeof(ApiClientRelationship) },
        { "ApiEntityProjectBudget", typeof(ApiClientProjectBudget) },
        { "ApiEntityResourceRoleAssignment", typeof(ApiClientAssignment) }
    };

    public MessageProcessor(IServiceCollection services)
    {
        _services = services;
        var serviceProvider = services.BuildServiceProvider(true);
        _scope = serviceProvider.CreateScope();
        _unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _configuration = _scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _logger = _scope.ServiceProvider.GetRequiredService<ILogger<MessageProcessor>>();
    }
    
    public async Task ProcessMessageAsync(string json)
    {
        try
        {
            if (json is null) return;

            var message = JsonSerializer.Deserialize<dynamic>(json);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            _logger.LogInformation($"New message received from Primavera Cloud is:\n {JsonSerializer.Serialize(message, options)}");
            var obj = JsonSerializer.Deserialize<ApiEntitySubscriptionView>(json);

            if (_entityObjectTypeToApiClientTypeMap.TryGetValue(obj!.EntityObjectType!, out var apiClientType))
            {
                var method = typeof(MessageProcessor)
                    .GetMethod("ProcessApiEntity", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(apiClientType);
                var task = (Task)method.Invoke(this, new object[] { obj, message! })!;
                await task!;
            }
            else
            {
                _logger.LogInformation($"Unknown EntityObjectType: {obj.EntityObjectType}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while processing the message: {ex.Message}");
            throw new BaseException($"An error occurred while processing the message: {ex.Message}");
        }
    }

    private async Task<EventNotification?> GetEventNotification(ApiEntitySubscriptionView obj)
    {
        var existEventNotification = await _unitOfWork.EventNotificationRepository.GetNoTracking()
            .FirstOrDefaultAsync(e => e.EntityObjectType!
                .Contains(obj.EntityObjectType!) && e.MessageType == "SUCCESS");

        return existEventNotification;
    }

    private async Task<Subscription?> GetSubscription(ApiEntitySubscriptionView obj)
    {
        var subscription = await _unitOfWork.SubscriptionRepository.GetNoTracking()
            .FirstOrDefaultAsync(e => e.EntityObjectType!.Contains(obj.EntityObjectType!));
        
        if (subscription is null && obj.EntityObjectType == "ApiEntityProjectBudget")
        {
            subscription = await _unitOfWork.SubscriptionRepository.GetNoTracking()
                .FirstOrDefaultAsync(e => e.EntityObjectType!.Contains("ApiEntityProject"));
        }
        
        return subscription;
    }

    private async Task<EventNotification> SaveEventNotification(ApiEntitySubscriptionView obj)
    {
        var subscription = await GetSubscription(obj);
        var eventTypeList = new List<string> { obj.EntityEventType! };
        var eventNotification = new EventNotification
        {
            SubscriptionId = subscription!.Id,
            Message = obj.MessageContent,
            MessageType = obj.MessageType,
            IsEnabled = subscription.IsEnabled,
            EntityObjectType = obj.EntityObjectType,
            EntityEventType = eventTypeList
        };

        var existEventNotification = await GetEventNotification(obj);
        if (existEventNotification != null) 
        {
            if (existEventNotification.EntityEventType.Contains(obj.EntityEventType!)) { return existEventNotification; }
            existEventNotification.EntityEventType.Add(obj.EntityEventType!);
            var updated = await _unitOfWork.EventNotificationRepository.Update(existEventNotification, new CancellationToken());
            return (updated as InsertedEvent<EventNotification>)?.Object!;
        }

        var entry = await _unitOfWork.EventNotificationRepository.Insert(eventNotification, new CancellationToken());
        return (entry as InsertedEvent<EventNotification>)?.Object!;
    }

    private async Task<Transaction> SaveTransaction(EventNotification eventNotification, ApiEntitySubscriptionView obj, dynamic message)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var entity = new Transaction
        {
            EventId = eventNotification.Id,
            EventType = obj.EntityObjectType,
            EventTimeStamp = DateTimeOffset.UtcNow,
            EventDetails = JsonSerializer.Serialize(message, options),
            SystemOrigin = _configuration.GetSection("PrimaveraCloudApi:HostName").Value!,
            InitiatingUser = _configuration.GetSection("PrimaveraCloudApi:UserName").Value!
        };

        var entry = await _unitOfWork.TransactionRepository.Insert(entity, new CancellationToken());
        return (entry as InsertedEvent<Transaction>)?.Object!;
    }

    private async Task CallApiClient<T>(Transaction transaction, dynamic message) where T : IHttpClientStrategy<HttpResponseMessage>
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var apiClientEntity = (T)Activator.CreateInstance(typeof(T), _services)!;
        var apiHttpClient = new ApiHttpClient(_scope!, apiClientEntity);
        var json = JsonSerializer.Serialize(message, options);
        _logger.LogInformation($"ExecuteRequest starts with :\n {json}");
        await apiHttpClient.ExecuteRequests(transaction, json);
        _logger.LogInformation($"ExecuteRequest completed");
    }
    
/// <summary>
/// This method invokes by ProcessMessageAsync method
/// </summary>
/// <param name="obj"></param>
/// <param name="message"></param>
/// <typeparam name="T"></typeparam>
    private async Task ProcessApiEntity<T>(ApiEntitySubscriptionView obj, dynamic message) where T : IHttpClientStrategy<HttpResponseMessage>
    {
        switch (obj.MessageType)
        {
            case "EVENT":
                var eventNotification = await GetEventNotification(obj);
                var transaction = await SaveTransaction(eventNotification, obj, message);
                await CallApiClient<T>(transaction, message);
                break;
            case "SUCCESS":
            case "ERROR":
                await SaveEventNotification(obj);
                break;
        }
    }
}