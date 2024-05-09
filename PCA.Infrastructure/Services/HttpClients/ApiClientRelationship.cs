namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientRelationship: IHttpClientStrategy<HttpResponseMessage>
{
    private EventNotification? _eventNotification;
    private readonly ApiHttpClient _httpClient;
    private readonly ILogger<ApiClientRelationship> _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public ApiClientRelationship(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        _httpClient = new ApiHttpClient(services);
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiClientRelationship>>();
    }
    
    public async Task GetDataAsync(EventNotification eventNotification, dynamic json)
    {
        _eventNotification = eventNotification;
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityRelationshipView>(json);
        var requestUri = $"/api/restapi/relationship/{apiEntity!.PrimaryKey}";
        
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
        var entity = new ActivityRelationship
        {
            EventId = _eventNotification!.Id,
            Json = eventDetails
        };
        
        await _unitOfWork.ActivityRelationshipRepository.Insert(entity, new CancellationToken());
    }
}