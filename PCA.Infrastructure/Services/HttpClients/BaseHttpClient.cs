namespace PCA.Infrastructure.Services.HttpClients;

public abstract class BaseHttpClient<T> : IHttpClientStrategy<HttpResponseMessage>
{
    protected EventNotification? _eventNotification;
    protected readonly ApiHttpClient _httpClient;
    protected readonly ILogger<T> _logger;
    protected readonly IUnitOfWork _unitOfWork;

    protected BaseHttpClient(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        _httpClient = new ApiHttpClient(services);
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<T>>();
    }

    public abstract Task GetDataAsync(EventNotification eventNotification, dynamic json);

    protected async Task SaveData(dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        await InsertEntity(eventDetails);
    }

    protected abstract Task InsertEntity(string eventDetails);
}