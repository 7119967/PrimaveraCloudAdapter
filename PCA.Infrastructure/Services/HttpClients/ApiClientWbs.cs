namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientWbs: IHttpClientStrategy<HttpResponseMessage>
{
    private EventNotification? _eventNotification;
    private readonly ApiHttpClient _httpClient;
    private readonly ILogger<ApiClientWbs> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ApiClientWbs(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        _httpClient = new ApiHttpClient(services);
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiClientWbs>>();
    }
    
    public async Task GetDataAsync(EventNotification eventNotification, dynamic json)
    {
        _eventNotification = eventNotification;
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityWbsView>(json);
        var requestUri = $"/api/restapi/wbs/{apiEntity!.PrimaryKey}";
        
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
        var entity = new Wbs
        {
            EventId = _eventNotification!.Id,
            Json = eventDetails
        };
        
        await _unitOfWork.WbsRepository.Insert(entity, new CancellationToken());
    }
}