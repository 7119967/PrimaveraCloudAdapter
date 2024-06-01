namespace PCA.Infrastructure.Services;

public class ApiEventProducer : IApiProducer
{
    private readonly string HOST_NAME;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly WebSocketClient _webSocketClient;
    private readonly ILogger<ApiEventProducer> _logger;
    
    public ApiEventProducer(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiEventProducer>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        HOST_NAME = configuration["PrimaveraCloudApi:HostName"]!;
        _webSocketClient = new WebSocketClient(services);
        var apiHttpClient = new ApiHttpClient(services);
        var authTokenResponse = apiHttpClient.GetAuthTokenDetails();
        _webSocketClient.SetCredentials(authTokenResponse!);
    }
    
    public async Task<string> SendAsync(string message)
    {
        try
        {
            var uri = $"wss://{HOST_NAME}/api/events";
            await _webSocketClient.ConnectAsync(uri);
            await _webSocketClient.SendMessageAsync(message);
            return await UpdateOrInsert(message);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in event subscription: {e}");
            throw new BaseException($"Error in event subscription: {e}");
        }
        finally
        {
          _webSocketClient.ReceiveMessageAsync(); 
        }
    }
    
    private async Task<string> UpdateOrInsert(string model, CancellationToken ctn = default)
    {
        Subscription entity;
        try
        {
            entity = JsonConvert.DeserializeObject<Subscription>(model)!;
            if (entity == null)
            {
                _logger.LogError($"Deserialized object is null");
                throw new InvalidOperationException("Deserialized object is null");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error deserializing model to Subscription {ex.Message}");
            throw new InvalidOperationException("Error deserializing model to Subscription", ex);
        }
        
        var result = _unitOfWork.SubscriptionRepository.GetTracking()
            .FirstOrDefault(e => e.EntityObjectType == entity.EntityObjectType);
       
        if (result != null)
        {
            await _unitOfWork.SubscriptionRepository.Update(result, ctn);
            return $"Subscription {result.EntityObjectType} was sent and updated successfully";
        }

        await _unitOfWork.SubscriptionRepository.Insert(entity, ctn);
        return $"Subscription {entity.EntityObjectType} was sent and created successfully";
    }
}