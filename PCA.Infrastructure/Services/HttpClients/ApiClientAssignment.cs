namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientAssignment: IHttpClientStrategy<HttpResponseMessage>
{
    private EventNotification? _eventNotification;
    private readonly ApiHttpClient _httpClient;
    private readonly ILogger<ApiClientAssignment> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ApiClientAssignment(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        _httpClient = new ApiHttpClient(services);
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiClientAssignment>>();
    }
    
    public async Task GetDataAsync(EventNotification eventNotification, dynamic json)
    {
        _eventNotification = eventNotification;
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityResourceAssignmentView>(json);
        var requestUri = $"/api/restapi/assignment/{apiEntity!.PrimaryKey}";
        
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
        var entity = new ResourceAssignment
        {
            EventId = _eventNotification!.Id,
            Json = eventDetails
        };
        
        await _unitOfWork.ResourceAssignmentRepository.Insert(entity, new CancellationToken());
    }
}