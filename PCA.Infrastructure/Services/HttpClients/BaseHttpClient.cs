namespace PCA.Infrastructure.Services.HttpClients;

public abstract class BaseHttpClient<T> : IHttpClientStrategy<HttpResponseMessage>
{
    protected readonly ILogger<T> Logger;
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly ApiHttpClient HttpClient;

    protected BaseHttpClient(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        HttpClient = new ApiHttpClient(services);
        UnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        Logger = scope.ServiceProvider.GetRequiredService<ILogger<T>>();
    }

    public abstract Task GetDataAsync(Transaction transaction, dynamic json);

    protected async Task SaveData(Transaction transaction, dynamic message)
    {
        var eventDetails = JsonConvert.SerializeObject(message, Formatting.Indented);
        await InsertEntity(transaction, eventDetails);
    }

    protected abstract Task InsertEntity(Transaction transaction, string eventDetails);
}