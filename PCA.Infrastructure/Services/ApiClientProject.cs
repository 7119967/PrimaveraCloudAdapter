namespace PCA.Infrastructure.Services;

public class ApiClientProject: IHttpClientStrategy<HttpResponseMessage>
{
    private readonly IServiceScope _scope;
    private readonly ApiHttpClient _httpClient;
    private readonly ILogger<ApiClientProject> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private EventNotification _eventNotification;
    
    public ApiClientProject(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        _scope = scope;
        _httpClient = new ApiHttpClient(services);
        _unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _logger = _scope.ServiceProvider.GetRequiredService<ILogger<ApiClientProject>>();
    }
    
    public async Task GetDataAsync(EventNotification eventNotification, dynamic json)
    {
        _eventNotification = eventNotification;
        ApiEntityProjectView apiEntityProject = JsonConvert.DeserializeObject<ApiEntityProjectView>(json);
        var requestUri = $"/api/restapi/project/{apiEntityProject!.PrimaryKey}";
        
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
        var entity = new Project
        {
            EventId = _eventNotification.Id,
            Json = eventDetails
        };
        
        await _unitOfWork.ProjectRepository.Insert(entity, new CancellationToken());
    }
}