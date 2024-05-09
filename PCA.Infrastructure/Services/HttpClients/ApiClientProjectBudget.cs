namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientProjectBudget: IHttpClientStrategy<HttpResponseMessage>
{
    private EventNotification? _eventNotification;
    private readonly ApiHttpClient _httpClient;
    private readonly ILogger<ApiClientProjectBudget> _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public ApiClientProjectBudget(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        _httpClient = new ApiHttpClient(services);
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiClientProjectBudget>>();
    }
    
    public async Task GetDataAsync(EventNotification eventNotification, dynamic json)
    {
        _eventNotification = eventNotification;
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityProjectBudgetView>(json);
        var requestUri = $"/api/restapi/projectBudget/{apiEntity!.PrimaryKey}";
        
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
        var entity = new ProjectBudget
        {
            EventId = _eventNotification!.Id,
            Json = eventDetails
        };
        
        await _unitOfWork.ProjectBudgetRepository.Insert(entity, new CancellationToken());
    }
}