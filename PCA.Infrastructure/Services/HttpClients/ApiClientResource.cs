namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientResource: IHttpClientStrategy<HttpResponseMessage>
{
    private EventNotification? _eventNotification;
    private readonly ApiHttpClient _httpClient;
    private readonly ILogger<ApiClientResource> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ApiClientResource(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        _httpClient = new ApiHttpClient(services);
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiClientResource>>(); 
    }
    
    public async Task GetDataAsync(EventNotification eventNotification, dynamic json)
    {
        _eventNotification = eventNotification;
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityResourceView>(json);
        var requestUri = $"/api/restapi/resource/{apiEntity!.PrimaryKey}";
        
        var response = await _httpClient.SendRequestAsync(requestUri);

        if (response == null)
        {
            _logger.LogDebug($"{GetType().Name} reports: The response of has no required data");
            return;
        }
        
        var jsonString = response.Content.ReadAsStringAsync().Result;
        var data = JsonConvert.DeserializeObject(jsonString);
        await SaveData(data);
    }

    private async Task SaveData(dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        var entity = new Resource
        {
            EventId = _eventNotification!.Id,
            Json = eventDetails
        };
        
        await _unitOfWork.ResourceRepository.Insert(entity, new CancellationToken());
    }
}