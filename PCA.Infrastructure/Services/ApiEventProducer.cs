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
        var authTokenResponse = apiHttpClient.GetAuthTokenDetails().Result;
        _webSocketClient.SetCredentials(authTokenResponse!);
    }

    public bool IsConnected => _webSocketClient.State == WebSocketState.Open;

    public async Task<string> SendAsync(string message)
    {
        try
        {
            var uri = $"wss://{HOST_NAME}/api/events";
            if (IsConnected)
            {
                await _webSocketClient.SendMessageAsync(message);
            }
            else
            {
                await _webSocketClient.ConnectAsync(uri);
                await _webSocketClient.SendMessageAsync(message); 
            }

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
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            entity = JsonSerializer.Deserialize<Subscription>(model, options)!;
            
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
        
        var result = await _unitOfWork.SubscriptionRepository.GetNoTracking()
            .FirstOrDefaultAsync(e => e.EntityObjectType == entity.EntityObjectType, ctn);
       
        if (result != null)
        {
            var updated = await _unitOfWork.SubscriptionRepository.Update(result, ctn);
            var updatedObject = updated as UpdatedEvent<Subscription>;
            return $"Subscription of {updatedObject!.Object.EntityObjectType} was sent and updated successfully";
        }

        var created =await _unitOfWork.SubscriptionRepository.Insert(entity, ctn);
        var createdObject = created as InsertedEvent<Subscription>;
        return $"Subscription of {createdObject!.Object.EntityObjectType} was sent and created successfully";
    }
}